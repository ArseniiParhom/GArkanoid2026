using Godot;

public partial class Paddle : CharacterBody2D
{
    [Export]
    public float Speed = 300.0f;

    private Sprite2D _sprite;
    private Ball _ball;

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _ball = GetNode<Ball>("../Ball");
    }

	// Called every physics frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        float direction = Input.GetAxis("MoveLeft", "MoveRight");

        Velocity = new Vector2(direction * Speed, 0);

        MoveAndSlide();

        float halfWidth = _sprite.GetRect().Size.X * _sprite.Scale.X / 2.0f;
        float viewportWidth = GetViewportRect().Size.X;

        Position = new Vector2(
            Mathf.Clamp(
                Position.X,
                halfWidth,
                viewportWidth - halfWidth
            ),
            Position.Y
        );

        if (Input.IsActionJustPressed("LaunchBall"))
        {
            _ball.Launch();
        }
    }
}
