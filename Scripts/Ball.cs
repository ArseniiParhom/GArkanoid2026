using Godot;
using Arkanoid.Persistence;

public partial class Ball : CharacterBody2D, ISaveable
{
    [Export] private Vector2 _direction = new Vector2(1, -1).Normalized();

    [Export] private float _speed = 200.0f;

    private bool _launched = false;

    private Paddle _paddle;
    private Vector2 _paddleOffset;

    public override void _Ready()
    {
        _paddle = GetNode<Paddle>("../Paddle");
        _paddleOffset = GlobalPosition - _paddle.GlobalPosition;
    }

    public void Launch()
    {
        if (_launched)
            return;

        _launched = true;
        _direction = _direction.Normalized();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_launched)
        {
            GlobalPosition = _paddle.GlobalPosition + _paddleOffset;
            return;
        }

        Vector2 motion = _direction * _speed * (float)delta;

        KinematicCollision2D collision = MoveAndCollide(motion);

        if (collision != null)
        {
            Node collider = collision.GetCollider() as Node;

            if (collider is Brick brick)
            {
                brick.Hit();
            }

            if (collider is Floor)
            {
                GameManager gameManager =
                    GetNode<GameManager>("/root/GameManager");

                gameManager.LoseLife();

                GD.Print($"Life lost! Lives remaining: {gameManager.Lives}");

                if (gameManager.Lives > 0)
                {
                    ResetBall();
                }
                else
                {
                    SetPhysicsProcess(false);
                }

                return;
            }

            Vector2 normal = collision.GetNormal();
            _direction = _direction.Bounce(normal).Normalized();
        }
    }

    public void ResetBall()
    {
        _launched = false;
        _direction = new Vector2(1, -1).Normalized();
        GlobalPosition = _paddle.GlobalPosition + _paddleOffset;
    }

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            { "positionX", GlobalPosition.X },
            { "positionY", GlobalPosition.Y },
            { "directionX", _direction.X },
            { "directionY", _direction.Y },
            { "speed", _speed },
            { "launched", _launched }
        };
    }

    public void LoadData(
        Godot.Collections.Dictionary<string, Variant> data)
    {
        GlobalPosition = new Vector2(
            data["positionX"].AsSingle(),
            data["positionY"].AsSingle()
        );

        _direction = new Vector2(
            data["directionX"].AsSingle(),
            data["directionY"].AsSingle()
        );

        _speed = data["speed"].AsSingle();
        _launched = data["launched"].AsBool();
    }
}