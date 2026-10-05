using Godot;

public static class Extensions
{
    public static Vector2 GetExtents(this Rect2 rect)
    {
        return rect.Size / 2;
    }

    public static Rect2 GetBoundingBox(this Sprite2D sprite)
    {
        Rect2 localRect = sprite.GetRect();

        Vector2 position =
            sprite.GlobalPosition + localRect.Position * sprite.GlobalScale;

        Vector2 size =
            localRect.Size * sprite.GlobalScale;

        return new Rect2(position, size);
    }
}
