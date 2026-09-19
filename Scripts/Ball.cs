using Godot;
using System;

public partial class Ball : Node2D
{
	[Export] private float _speed = 10f;

	public float Speed { 
		get { return _speed; }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;

		Vector2 input = Input.GetVector("Left", "Right", "Up", "Down");

		GD.Print($"Input: {input}");

		Position += input * _speed * deltaTime;

		GD.Print($"Position: {Position}");
	}

	// <summary>
	// Called every physics frame. 'delta' is the elapsed time since the previous frame.
	// </summary>
	// <param name="delta">The elapsed time since the previous frame.</param>
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
}
