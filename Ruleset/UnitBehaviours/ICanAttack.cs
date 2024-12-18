public interface ICanAttack : IUnit
{
    public bool CanAttackRanged();

    public AttackType GetAttackType(IAttackable attackable);

    public AttackParameters CalculateParameters(IAttackable target, bool allowRanged = true, bool isCounterattack = false);

    public double CalculateDamageFromParameters(AttackParameters parameters);

    public void AttackFromParameters(IAttackable target, AttackParameters parameters, bool triggerEvents = true);

    /// <summary>
    /// Attacks the target.
    /// </summary>
    /// <param name="attackable">Target.</param>
    /// <param name="multiplier">Multiplier to damage.</param>
    /// <returns></returns>
    public bool Attack(IAttackable target, bool allowRanged = true, bool isCounterattack = false, bool triggerEvents = true);
}