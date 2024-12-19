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
}