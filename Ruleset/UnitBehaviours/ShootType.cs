public enum AttackType
{
    None,
    Melee,
    MeleeWithPenalty,
    RangedWeak,
    RangedStrong
}

public static class AttackTypeExtensions
{
    public static bool IsMelee(this AttackType type) => type == AttackType.Melee || type == AttackType.MeleeWithPenalty;

    public static bool IsRanged(this AttackType type) => type == AttackType.RangedWeak || type == AttackType.RangedStrong;

    public static double GetMultiplier(this AttackType type) => type switch
    {
        AttackType.Melee => 1.0,
        AttackType.MeleeWithPenalty => 0.5,
        AttackType.RangedWeak => 0.5,
        AttackType.RangedStrong => 1.0,
        _ => 0.0
    };
}