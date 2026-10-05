using Godot;

public static class CustomPhysics
{
    public class Hit
    {
        public Vector2 Point { get; set; }
        public Vector2 Normal { get; set; }
        public Vector2 Penetration { get; set; }
    }

    // Point vs Rectangle
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

    // Circle vs Rectangle
    public static Hit Intersects(Rect2 rect, Vector2 center, float radius)
    {
        Hit pointHit = Intersects(rect, center);

        if (pointHit != null)
        {
            pointHit.Penetration += pointHit.Normal * radius;
            return pointHit;
        }

        Vector2 closestPoint = new Vector2(
            Mathf.Clamp(center.X, rect.Position.X, rect.End.X),
            Mathf.Clamp(center.Y, rect.Position.Y, rect.End.Y)
        );

        Vector2 delta = center - closestPoint;
        float distanceSquared = delta.LengthSquared();

        if (distanceSquared > radius * radius)
        {
            return null;
        }

        float distance = Mathf.Sqrt(distanceSquared);

        Vector2 normal = delta.Normalized();
        float penetrationDepth = radius - distance;

        return new Hit
        {
            Point = closestPoint,
            Normal = normal,
            Penetration = normal * penetrationDepth
        };
    }

    // Bounce / Reflection
    public static Vector2 Bounce(Vector2 direction, Vector2 normal)
    {
        Vector2 parallelComponent = direction.Dot(normal) * normal;
        Vector2 perpendicularComponent = direction - parallelComponent;

        return perpendicularComponent - parallelComponent;
    }
}
