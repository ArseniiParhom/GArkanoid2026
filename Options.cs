using Godot;

public partial class Options : Control
{
    private Button _closeButton;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        _closeButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/CloseButton"
        );

        _closeButton.Pressed += OnCloseButtonPressed;
    }

    private void OnCloseButtonPressed()
    {
        _gameManager.CloseOptions();
    }
}
