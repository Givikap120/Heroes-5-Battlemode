using Godot;

public struct MoveResult
{
    public Vector2I Before;
    public Vector2I After;

    public static bool operator ==(MoveResult left, MoveResult right) =>
        left.Before == right.Before && left.After == right.After;

    public static bool operator !=(MoveResult left, MoveResult right) =>
        !(left == right);

    public override bool Equals(object obj) =>
        obj is MoveResult other && this == other;

    public override int GetHashCode() =>
        Before.GetHashCode() ^ After.GetHashCode();
}
