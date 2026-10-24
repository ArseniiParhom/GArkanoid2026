using Godot;

public partial class MainMenu : Control
{
    private Button _startButton;
    private Button _optionsButton;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        _startButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/StartButton"
        );

        _optionsButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/OptionsButton"
        );

        _startButton.Pressed += OnStartButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        _gameManager.StartGame();
    }

    private void OnOptionsButtonPressed()
    {
        _gameManager.ChangeState(GameManager.GameState.Options);
    }
}
