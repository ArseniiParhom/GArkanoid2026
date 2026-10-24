using Godot;

public partial class GameOver : Control
{
    private Button _restartButton;
    private Button _mainMenuButton;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        _restartButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/RestartButton"
        );

        _mainMenuButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/MainMenuButton"
        );

        _restartButton.Pressed += OnRestartButtonPressed;
        _mainMenuButton.Pressed += OnMainMenuButtonPressed;
    }

    private void OnRestartButtonPressed()
    {
        _gameManager.StartGame();
    }

    private void OnMainMenuButtonPressed()
    {
        _gameManager.ChangeState(GameManager.GameState.MainMenu);
    }
}
