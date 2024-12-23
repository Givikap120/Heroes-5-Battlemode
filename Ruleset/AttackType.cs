using Godot;

public enum AttackType
{
    None,
    Melee,
    MeleeWithPenalty,
    RangedWeak,
    RangedStrong,
    Hero
}

public static class AttackTypeExtensions
{
    public static bool IsMelee(this AttackType type) => type == AttackType.Melee || type == AttackType.MeleeWithPenalty;

    public static bool IsRanged(this AttackType type) => type == AttackType.Hero || type.IsRangedShot();

    public static bool IsRangedShot(this AttackType type) => type == AttackType.RangedWeak || type == AttackType.RangedStrong;

    public static double GetMultiplier(this AttackType type) => type switch
    {
        AttackType.None => 0.0,
        AttackType.Melee => 1.0,
        AttackType.MeleeWithPenalty => 0.5,
        AttackType.RangedWeak => 0.5,
        AttackType.RangedStrong => 1.0,
        _ => 1.0
    };

    public static Cursor GetCursor(this AttackType type)
    {
        return type switch
        {
            AttackType.None => Cursor.CantCast,
            AttackType.Melee => Cursor.AttackUp,
            AttackType.MeleeWithPenalty => Cursor.AttackUp,
            AttackType.RangedWeak => Cursor.ShootPenalty,
            AttackType.RangedStrong => Cursor.Shoot,
            AttackType.Hero => Cursor.Shoot,
            _ => Cursor.Default,
        };
    }

    public static Cursor GetCursor(this AttackType type, Vector2I directionDelta)
    {
        if (!type.IsMelee())
            return type.GetCursor();

        return directionDelta.X switch
        {
            1 => directionDelta.Y switch
            {
                0 => Cursor.AttackLeft,
                1 => Cursor.AttaclLeftUp,
                -1 => Cursor.AttaclLeftDown,
                _ => Cursor.CantCast
            },
            -1 => directionDelta.Y switch
            {
                0 => Cursor.AttackRight,
                1 => Cursor.AttackRightUp,
                -1 => Cursor.AttackRightDown,
                _ => Cursor.CantCast
            },
            0 => directionDelta.Y switch
            {
                1 => Cursor.AttackUp,
                -1 => Cursor.AttackDown,
                _ => Cursor.CantCast
            },
            _ => Cursor.CantCast
        };
    }
}