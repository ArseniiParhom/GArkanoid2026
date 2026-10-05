using Godot;

public partial class CustomBall : Sprite2D
{
    [Export] private Vector2 _direction = new Vector2(1, -1).Normalized();
    [Export] private float _speed = 200.0f;

    private Sprite2D _topWall;
    private Sprite2D _bottomWall;
    private Sprite2D _leftWall;
    private Sprite2D _rightWall;

    private float _radius;

    public override void _Ready()
    {
        _topWall = GetNode<Sprite2D>("../TopWall");
        _bottomWall = GetNode<Sprite2D>("../BottomWall");
        _leftWall = GetNode<Sprite2D>("../LeftWall");
        _rightWall = GetNode<Sprite2D>("../RightWall");

        _radius = Texture.GetSize().X * Mathf.Abs(GlobalScale.X) / 2.0f;
    }

    public override void _Process(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;

        CheckCollision(_topWall);
        CheckCollision(_bottomWall);
        CheckCollision(_leftWall);
        CheckCollision(_rightWall);
    }

    private void CheckCollision(Sprite2D wall)
    {
        CustomPhysics.Hit hit =
            CustomPhysics.Intersects(
                wall.GetBoundingBox(),
                GlobalPosition,
                _radius
            );

        if (hit == null)
            return;

        GlobalPosition += hit.Penetration;

        _direction =
            CustomPhysics.Bounce(_direction, hit.Normal).Normalized();
    }
}
