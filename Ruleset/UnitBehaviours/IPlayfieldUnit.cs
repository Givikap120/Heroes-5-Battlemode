using Godot;

public interface IPlayfieldUnit : IUnit
{
    public Bindable<Vector2I> CoordsBindable { get; set; }
    public Vector2I Coords { get; set; }
    public bool IsLargeUnit { get; }

    public bool CanBePlacedOnTile(Vector2I tile)
    {
        if (GameHandler.AnyHandler.IsTileOccupied(tile, this))
            return false;

        if (IsLargeUnit)
        {
            foreach (var childTile in CoordExtensions.GetPartialSquare(tile))
            {
                if (GameHandler.AnyHandler.IsTileOccupied(childTile, this))
                    return false;
            }
        }

        return true;
    }
}

public static class PlayfieldUnitExtensions
{
    public static Vector2I GetTheClosestPointTo(this IPlayfieldUnit unit, Vector2I point)
    {
        if (!unit.IsLargeUnit) return unit.Coords;

        Vector2I unitPos = unit.Coords;

        if (point.X > unitPos.X) unitPos.X++;
        if (point.Y > unitPos.Y) unitPos.Y++;

        return unitPos;
    }

    public static bool IsNeighboring(this IPlayfieldUnit unit, IPlayfieldUnit other)
    {
        var thisClosestPoint = unit.GetTheClosestPointTo(other.Coords);
        var otherClosestPoint = other.GetTheClosestPointTo(thisClosestPoint);
        thisClosestPoint = unit.GetTheClosestPointTo(otherClosestPoint);

        return thisClosestPoint.IsNeighboring(otherClosestPoint);
    }
    public static double DistanceTo(this IPlayfieldUnit unit, IPlayfieldUnit other)
    {
        var thisClosestPoint = unit.GetTheClosestPointTo(other.Coords);
        var otherClosestPoint = other.GetTheClosestPointTo(thisClosestPoint);
        thisClosestPoint = unit.GetTheClosestPointTo(otherClosestPoint);

        return (thisClosestPoint - otherClosestPoint).Length();
    }

    public static double DistanceTo(this IPlayfieldUnit unit, Vector2I point)
    {
        var thisClosestPoint = unit.GetTheClosestPointTo(point);
        return (thisClosestPoint - point).Length();
    }

    public static Vector2 GetCenter(this IPlayfieldUnit unit) => unit.IsLargeUnit ? unit.Coords + new Vector2(0.5f, 0.5f) : unit.Coords;
}
