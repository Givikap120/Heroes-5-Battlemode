public interface ICanAttack : IUnit
{
    public bool CanAttackRanged();

    public AttackType GetAttackType(IAttackable attackable);

    public AttackParameters CalculateParameters(IAttackable target, bool triggerEvents, bool isCounterattack = false);

    public double CalculateDamageFromParameters(AttackParameters parameters);

    public void AttackFromParameters(IAttackable target, AttackParameters parameters);

    /// <summary>
    /// Attacks the target.
    /// </summary>
    /// <param name="attackable">Target.</param>
    /// <param name="multiplier">Multiplier to damage.</param>
    /// <returns></returns>
    public bool Attack(IAttackable target, bool triggerEvents, bool isCounterattack = false);
}