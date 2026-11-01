using Godot;

public partial class PauseOnly : Node
{
    public override void _Ready()
    {
        GameManager gameManager =
            GetNode<GameManager>("/root/GameManager");

        Button parentButton = GetParent<Button>();

        bool isPause =
            gameManager.CurrentState == GameManager.GameState.Pause;

        parentButton.Visible = isPause;
        parentButton.Disabled = !isPause;
    }
}
