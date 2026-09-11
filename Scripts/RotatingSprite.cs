using Godot;
using System;

namespace GA.GArkanoid
{
public partial class RotatingSprite : Sprite2D
{
	[Export] private int _speed = 400;
	[Export] private float _angularSpeed = Mathf.Pi;
	public RotatingSprite()
	{
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
		{
			GD.Print("RotatingSprite ready");
		}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotate(_angularSpeed * (float)delta);
	}
}
}
