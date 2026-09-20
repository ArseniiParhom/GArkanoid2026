using Godot;

public partial class Ball : Node2D
{
    [Export]
    public float Speed = 200.0f;

    private Vector2 _velocity = Vector2.Zero;
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

        _velocity = new Vector2(1, -1).Normalized() * Speed;
    }

    public override void _Process(double delta)
    {
        if (!_launched)
        {
            GlobalPosition = _paddle.GlobalPosition + _paddleOffset;
            return;
        }

        Position += _velocity * (float)delta;
    }
}
