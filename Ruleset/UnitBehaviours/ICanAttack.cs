using System.Collections.Generic;

public interface ICanAttack : IUnit
{
    public bool CanAttackRanged(IEnumerable<IPlayfieldUnit> units);

    public enum ShootType
    {
        None,
        Melee,
        Weak,
        Strong
    }

    public ShootType CanShootTarget(IAttackable attackable);

    /// <summary>
    /// Attacks the target.
    /// </summary>
    /// <param name="attackable">Target.</param>
    /// <param name="multiplier">Multiplier to damage.</param>
    /// <returns></returns>
    public bool Attack(IAttackable attackable, bool allowRanged = true, bool allowCounterattack = true);
}
