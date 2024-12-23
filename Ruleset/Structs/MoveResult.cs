using Godot;

public struct MoveResult
{
    public ICanMove Actor;
    public Vector2I Before;
    public Vector2I After;

    public static bool operator ==(MoveResult left, MoveResult right) =>
        left.Before == right.Before && left.After == right.After;

    public static bool operator !=(MoveResult left, MoveResult right) =>
        !(left == right);

    public override readonly bool Equals(object? obj) =>
        obj is MoveResult other && this == other;

    public override readonly int GetHashCode() =>
        Before.GetHashCode() ^ After.GetHashCode();
}
