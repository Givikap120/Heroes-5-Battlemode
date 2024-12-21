public interface IAttackable : IPlayfieldUnit
{
    public double MaxHP { get; }
    public double TotalHP { get; }
    public double Defense { get; }

    public int Tier { get; }

    public AttackResult CalculateAttackResult(double damage, AttackType attackType);

    public void ApplyDamage(AttackResult result, bool triggerEvents);

    public AttackResult TakeDamage(double damage, AttackType attackType, bool triggerEvents)
    {
        AttackResult result = CalculateAttackResult(damage, attackType);
        ApplyDamage(result, triggerEvents);
        return result;
    }

    public int AttackedOnThisTurn { get; set; }

    public bool CanCounterattack();

    public bool WillCounterattack(ICanAttack attacker)
    {
        // Can't attack - can't counterattack
        if (this is not ICanAttack)
            return false;

        // Only can counterattack to playfield units
        if (attacker is not IPlayfieldUnit playfieldUnit)
            return false;

        bool isNeighbor = playfieldUnit.IsNeighboring(this);

        // Can only counterattack neighbors and only once in the turn
        bool result = isNeighbor && CanCounterattack();

        // For Abilities OfType<IApplicableToCounterAttack> .Apply(result)
        return result;
    }
}
