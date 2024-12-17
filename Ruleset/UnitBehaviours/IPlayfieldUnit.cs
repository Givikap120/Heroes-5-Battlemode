using Godot;

public interface IPlayfieldUnit : IUnit
{
    public Vector2I Coords { get; set; }
}

public static class PlayfieldUnitExtensions
{
    public static bool IsNeighboring(this IPlayfieldUnit unit, IPlayfieldUnit other) => unit.Coords.IsNeighboring(other.Coords);
    public static double DistanceTo(this IPlayfieldUnit unit, IPlayfieldUnit other) => (unit.Coords - other.Coords).Length();
}
