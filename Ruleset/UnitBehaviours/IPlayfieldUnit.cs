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
    public static Vector2I GetTheClosestPointTo(Vector2I coords, bool isLargeCoords, Vector2I point)
    {
        if (!isLargeCoords) return coords;

        Vector2I unitPos = coords;

        if (point.X > unitPos.X) unitPos.X++;
        if (point.Y > unitPos.Y) unitPos.Y++;

        return unitPos;
    }

    public static double DistanceBetween(Vector2I point1, bool isLarge1, Vector2I point2, bool isLarge2)
    {
        var thisClosestPoint = GetTheClosestPointTo(point1, isLarge1, point2);
        var otherClosestPoint = GetTheClosestPointTo(point2, isLarge2, thisClosestPoint);
        thisClosestPoint = GetTheClosestPointTo(point1, isLarge1, otherClosestPoint);

        return (thisClosestPoint - otherClosestPoint).Length();
    }

    public static bool IsNeighboring(Vector2I point1, bool isLarge1, Vector2I point2, bool isLarge2)
    {
        var thisClosestPoint = GetTheClosestPointTo(point1, isLarge1, point2);
        var otherClosestPoint = GetTheClosestPointTo(point2, isLarge2, thisClosestPoint);
        thisClosestPoint = GetTheClosestPointTo(point1, isLarge1, otherClosestPoint);

        return thisClosestPoint.IsNeighboring(otherClosestPoint);
    }

    public static Vector2I GetTheClosestPointTo(this IPlayfieldUnit unit, Vector2I point) => GetTheClosestPointTo(unit.Coords, unit.IsLargeUnit, point);

    public static bool IsNeighboring(this IPlayfieldUnit unit, IPlayfieldUnit other) => IsNeighboring(unit.Coords, unit.IsLargeUnit, other.Coords, other.IsLargeUnit);

    public static double DistanceTo(this IPlayfieldUnit unit, IPlayfieldUnit other) => DistanceBetween(unit.Coords, unit.IsLargeUnit, other.Coords, other.IsLargeUnit);

    public static double DistanceTo(this IPlayfieldUnit unit, Vector2I point)
    {
        var thisClosestPoint = unit.GetTheClosestPointTo(point);
        return (thisClosestPoint - point).Length();
    }

    public static Vector2 GetCenter(this IPlayfieldUnit unit) => unit.IsLargeUnit ? unit.Coords + new Vector2(0.5f, 0.5f) : unit.Coords;
}
