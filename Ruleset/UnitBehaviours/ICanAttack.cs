public interface ICanAttack : IUnit
{
    public bool CanAttackRanged { get; }

    public enum ShootType
    {
        None,
        Weak,
        Strong
    }

    public ShootType CanShootTarget(IAttackable attackable);

    public bool AttackRanged(IAttackable attackable, ShootType type)
    {
        if (!CanAttackRanged || type == ShootType.None) return false;

        double multiplier = type switch
        {
            ShootType.Weak => 0.5,
            ShootType.Strong => 1.0,
            _ => 0.0
        };

        return AttackInternal(attackable, multiplier);
    }

    /// <summary>
    /// This method is NOT intended to be used as standalone method.
    /// Use AttackRanged or AttackAndMove instead.
    /// </summary>
    /// <param name="attackable">Target.</param>
    /// <param name="multiplier">Multiplier to damage.</param>
    /// <returns></returns>
    public bool AttackInternal(IAttackable attackable, double multiplier = 1.0);
}
