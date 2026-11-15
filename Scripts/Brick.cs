using Godot;

public partial class Brick : StaticBody2D
{
    [Export] public int Points { get; set; } = 100;

    private bool _isDestroyed = false;

    public void Hit()
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;

        GameManager gameManager =
            GetNode<GameManager>("/root/GameManager");

        gameManager.AddScore(Points);

        GD.Print(
            $"Brick hit! +{Points} points. Total score: {gameManager.Score}"
        );

        Node bricksContainer = GetParent();

        QueueFree();

        // Check whether any active bricks remain
        foreach (Node child in bricksContainer.GetChildren())
        {
            if (child is Brick brick && !brick.IsQueuedForDeletion())
                return;
        }

        GD.Print("All bricks destroyed! Victory!");

        // Change scenes after the physics callback finishes
        Callable.From(() =>
        {
            if (gameManager.CurrentState == GameManager.GameState.Level)
            {
                gameManager.ChangeState(GameManager.GameState.Victory);
            }
        }).CallDeferred();
    }
}
