using Godot;
using Arkanoid.Persistence;

public partial class Brick : StaticBody2D, ISaveable
{
    [Export] public int Points { get; set; } = 100;

    private bool _isDestroyed = false;

    public void Hit()
    {
        if (_isDestroyed)
            return;

        SetDestroyed(true);

        GameManager gameManager =
            GetNode<GameManager>("/root/GameManager");

        gameManager.AddScore(Points);

        GD.Print(
            $"Brick hit! +{Points} points. Total score: {gameManager.Score}"
        );

        Node bricksContainer = GetParent();

        foreach (Node child in bricksContainer.GetChildren())
        {
            if (child is Brick brick && !brick._isDestroyed)
                return;
        }

        GD.Print("All bricks destroyed! Victory!");

        Callable.From(() =>
        {
            if (gameManager.CurrentState == GameManager.GameState.Level)
            {
                gameManager.ChangeState(GameManager.GameState.Victory);
            }
        }).CallDeferred();
    }

    private void SetDestroyed(bool destroyed)
    {
        _isDestroyed = destroyed;

        Visible = !destroyed;

        GetNode<CollisionShape2D>("CollisionShape2D")
            .SetDeferred("disabled", destroyed);
    }

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            { "isDestroyed", _isDestroyed }
        };
    }

    public void LoadData(
        Godot.Collections.Dictionary<string, Variant> data)
    {
        SetDestroyed(data["isDestroyed"].AsBool());
    }
}
