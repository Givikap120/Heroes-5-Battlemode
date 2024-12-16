public interface IApplicableBeforeAttack : IAbility
{
    /// <summary>
    /// Applied to target right before attack.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    /// <param name="isRanged"></param>
    /// <param name="isCounterAttack"></param>
    /// <returns>Is counterattack possible after this or no</returns>
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters);
}