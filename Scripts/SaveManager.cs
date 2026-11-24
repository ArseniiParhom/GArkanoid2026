using Godot;
using System.Threading.Tasks;
using SaveDictionary = Godot.Collections.Dictionary<string, Godot.Variant>;

namespace Arkanoid.Persistence;

public static class SaveManager
{
    private const string SavePath = "user://savegame.json";

    public static bool HasSave()
    {
        return FileAccess.FileExists(SavePath);
    }

    public static void SaveGame(GameManager gameManager)
    {
        Node level = gameManager.GetTree().CurrentScene;

        if (level == null)
            return;

        Ball ball = level.GetNodeOrNull<Ball>("Ball");
        Paddle paddle = level.GetNodeOrNull<Paddle>("Paddle");
        Node bricksContainer = level.GetNodeOrNull<Node>("Bricks");

        if (ball == null || paddle == null || bricksContainer == null)
        {
            GD.PushWarning("Saving is only available during gameplay.");
            return;
        }

        SaveDictionary bricksData = new SaveDictionary();

        foreach (Node child in bricksContainer.GetChildren())
        {
            if (child is Brick brick)
            {
                bricksData[brick.Name.ToString()] = brick.SaveData();
            }
        }

        SaveDictionary saveData = new SaveDictionary
        {
            { "gameManager", gameManager.SaveData() },
            { "ball", ball.SaveData() },
            { "paddle", paddle.SaveData() },
            { "bricks", bricksData }
        };

        string json = Json.Stringify(saveData, "\t");

        using var file = FileAccess.Open(
            SavePath,
            FileAccess.ModeFlags.Write
        );

        if (file == null)
        {
            GD.PushError("Could not open save file.");
            return;
        }

        file.StoreString(json);

        GD.Print("Game saved successfully.");
    }

    public static async Task LoadGame(GameManager gameManager)
    {
        if (!HasSave())
        {
            GD.PushWarning("No saved game found.");
            return;
        }

        string json;

        using (var file = FileAccess.Open(
            SavePath,
            FileAccess.ModeFlags.Read))
        {
            if (file == null)
            {
                GD.PushError("Could not read save file.");
                return;
            }

            json = file.GetAsText();
        }

        Variant parsedData = Json.ParseString(json);

        if (parsedData.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError("Invalid save file.");
            return;
        }

        SaveDictionary saveData =
            parsedData.AsGodotDictionary<string, Variant>();

        if (!saveData.ContainsKey("gameManager") ||
            !saveData.ContainsKey("ball") ||
            !saveData.ContainsKey("paddle") ||
            !saveData.ContainsKey("bricks"))
        {
            GD.PushError("Save file is incomplete.");
            return;
        }

        SaveDictionary managerData =
            saveData["gameManager"]
                .AsGodotDictionary<string, Variant>();

        GameManager.GameState savedState =
            (GameManager.GameState)managerData["state"].AsInt32();

        if (savedState != GameManager.GameState.Level)
        {
            GD.PushError("Invalid saved game state.");
            return;
        }

        SceneTree tree = gameManager.GetTree();

        gameManager.ChangeState(savedState);

        if (tree.CurrentScene != null)
        {
            GD.PushError("Could not reload the saved level.");
            return;
        }

        await gameManager.ToSignal(
            tree,
            SceneTree.SignalName.SceneChanged
        );

        Node level = tree.CurrentScene;

        Ball ball = level.GetNode<Ball>("Ball");
        Paddle paddle = level.GetNode<Paddle>("Paddle");
        Node bricksContainer = level.GetNode<Node>("Bricks");

        gameManager.LoadData(managerData);

        paddle.LoadData(
            saveData["paddle"]
                .AsGodotDictionary<string, Variant>()
        );

        ball.LoadData(
            saveData["ball"]
                .AsGodotDictionary<string, Variant>()
        );

        SaveDictionary bricksData =
            saveData["bricks"]
                .AsGodotDictionary<string, Variant>();

        foreach (Node child in bricksContainer.GetChildren())
        {
            if (child is Brick brick)
            {
                string brickName = brick.Name.ToString();

                if (bricksData.ContainsKey(brickName))
                {
                    brick.LoadData(
                        bricksData[brickName]
                            .AsGodotDictionary<string, Variant>()
                    );
                }
            }
        }

        GD.Print("Game loaded successfully.");
    }
}
