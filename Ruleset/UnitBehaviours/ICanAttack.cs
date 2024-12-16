using System.Collections.Generic;

public interface ICanAttack : IUnit
{
    public bool CanAttackRanged(IEnumerable<IPlayfieldUnit> units);

    public AttackType CanShootTarget(IAttackable attackable);

    /// <summary>
    /// Attacks the target.
    /// </summary>
    /// <param name="attackable">Target.</param>
    /// <param name="multiplier">Multiplier to damage.</param>
    /// <returns></returns>
    public bool Attack(IAttackable attackable, bool allowRanged = true, bool isCounterattack = false);
}
