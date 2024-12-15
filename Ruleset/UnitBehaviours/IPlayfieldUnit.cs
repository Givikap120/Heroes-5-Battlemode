using Godot;

public interface IPlayfieldUnit : IUnit
{
    public Vector2I Coords { get; set; }
}
