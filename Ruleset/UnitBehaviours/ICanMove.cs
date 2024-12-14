using Godot;

public interface ICanMove
{
    public Vector2I Coords { get; set; }
    public double Speed { get; }

    public bool CanMoveTo(Vector2I target)
    {
        double maxDistance = Speed * Speed;
        double distanceSquared = target.DistanceSquaredTo(Coords);
        return distanceSquared > 0 && distanceSquared <= maxDistance;
    }

    public bool MoveTo(Vector2I target)
    {
        if (!CanMoveTo(target)) return false;
        Coords = target;
        return true;
    }
}
