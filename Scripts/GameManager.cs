using Godot;

public partial class GameManager : Node
{
    public int Score { get; private set; } = 0;

    public int Lives { get; private set; } = 3;

    [Signal] public delegate void ScoreChangedEventHandler(int newScore);

    [Signal] public delegate void LivesChangedEventHandler(int newLives);

    public void AddScore(int amount)
    {
        Score += amount;
        EmitSignal(SignalName.ScoreChanged, Score);
    }

    public void LoseLife()
	{
		if (Lives <= 0)
			return;

		Lives--;

		EmitSignal(SignalName.LivesChanged, Lives);
	}
}
