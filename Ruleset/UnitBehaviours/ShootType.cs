public enum ShootType
{
    None,
    Melee,
    MeleeWithPenalty,
    Weak,
    Strong
}

public static class ShootTypeExtensions
{
    public static bool IsMelee(this ShootType type) => type == ShootType.Melee || type == ShootType.MeleeWithPenalty;

    public static bool IsRanged(this ShootType type) => type == ShootType.Weak || type == ShootType.Strong;

    public static double GetMultiplier(this ShootType type) => type switch
    {
        ShootType.Melee => 1.0,
        ShootType.MeleeWithPenalty => 0.5,
        ShootType.Weak => 0.5,
        ShootType.Strong => 1.0,
        _ => 0.0
    };
}