public interface IAttackable : IPlayfieldUnit
{
    public double TotalHP { get; }
    public double Defense { get; }

    public AttackResult CalculateAttackResult(double damage, AttackType attackType);

    public void ApplyDamage(AttackResult result, bool triggerEvents = true);

    public AttackResult TakeDamage(double damage, AttackType attackType, bool triggerEvents = true)
    {
        AttackResult result = CalculateAttackResult(damage, attackType);
        ApplyDamage(result, triggerEvents);
        return result;
    }

    public int AttackedOnThisTurn { get; set; }

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
        bool result = isNeighbor && AttackedOnThisTurn == 0;

        // For Abilities OfType<IApplicableToCounterAttack> .Apply(result)
        return result;
    }

    public void SaveState();
    public void LoadState(bool silent = false);
}
