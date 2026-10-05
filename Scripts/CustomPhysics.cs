using Godot;

public static class CustomPhysics
{
    public class Hit
    {
        public Vector2 Point { get; set; }
        public Vector2 Normal { get; set; }
        public Vector2 Penetration { get; set; }
    }

    public static Hit Intersects(Rect2 rect, Vector2 point)
    {
        Vector2 center = rect.GetCenter();
        Vector2 extents = rect.GetExtents();

        Vector2 delta = point - center;

        float penetrationX = extents.X - Mathf.Abs(delta.X);
        float penetrationY = extents.Y - Mathf.Abs(delta.Y);

        if (penetrationX < 0 || penetrationY < 0)
        {
            return null;
        }

        if (penetrationX < penetrationY)
        {
            float signX = Mathf.Sign(delta.X);

            return new Hit
            {
                Normal = new Vector2(signX, 0),
                Penetration = new Vector2(penetrationX * signX, 0),
                Point = new Vector2(
                    center.X + extents.X * signX,
                    point.Y
                )
            };
        }
        else
        {
            float signY = Mathf.Sign(delta.Y);

            return new Hit
            {
                Normal = new Vector2(0, signY),
                Penetration = new Vector2(0, penetrationY * signY),
                Point = new Vector2(
                    point.X,
                    center.Y + extents.Y * signY
                )
            };
        }
    }

	public static Vector2 Bounce(Vector2 direction, Vector2 normal)
	{
		Vector2 parallelComponent = direction.Dot(normal) * normal;
		Vector2 perpendicularComponent = direction - parallelComponent;

		return perpendicularComponent - parallelComponent;
	}
}
