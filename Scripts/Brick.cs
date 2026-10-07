using Godot;

public partial class Brick : StaticBody2D
{
    [Export] public int Points { get; set; } = 100;

    public void Hit()
	{
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");

		gameManager.AddScore(Points);

		GD.Print($"Brick hit! +{Points} points. Total score: {gameManager.Score}");

		QueueFree();
	}
}
