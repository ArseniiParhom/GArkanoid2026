using Godot;
using Arkanoid.Persistence;

public partial class MainMenu : Control
{
    private Button _startButton;
    private Button _loadButton;
    private Button _optionsButton;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        string basePath = "CenterContainer/VBoxContainer/";

        _startButton = GetNode<Button>(basePath + "StartButton");
        _loadButton = GetNode<Button>(basePath + "LoadButton");
        _optionsButton = GetNode<Button>(basePath + "OptionsButton");

        _startButton.Pressed += OnStartButtonPressed;
        _loadButton.Pressed += OnLoadButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;

        _loadButton.Disabled = !SaveManager.HasSave();
    }

    private void OnStartButtonPressed()
    {
        _gameManager.StartGame();
    }

    private async void OnLoadButtonPressed()
    {
        _loadButton.Disabled = true;

        try
        {
            await SaveManager.LoadGame(_gameManager);
        }
        catch (System.Exception exception)
        {
            GD.PushError(exception.ToString());
        }
        finally
        {
            if (GodotObject.IsInstanceValid(_loadButton))
            {
                _loadButton.Disabled = !SaveManager.HasSave();
            }
        }
    }

    private void OnOptionsButtonPressed()
    {
        _gameManager.ChangeState(GameManager.GameState.Options);
    }
}
