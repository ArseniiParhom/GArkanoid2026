using Godot;
using Arkanoid.Persistence;

public partial class Paddle : CharacterBody2D, ISaveable
{
    [Export]
    public float Speed = 300.0f;

    private Sprite2D _sprite;
    private Ball _ball;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _ball = GetNode<Ball>("../Ball");
    }

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

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            { "positionX", GlobalPosition.X },
            { "positionY", GlobalPosition.Y },
            { "speed", Speed }
        };
    }

    public void LoadData(
        Godot.Collections.Dictionary<string, Variant> data)
    {
        GlobalPosition = new Vector2(
            data["positionX"].AsSingle(),
            data["positionY"].AsSingle()
        );

        Speed = data["speed"].AsSingle();
    }
}
