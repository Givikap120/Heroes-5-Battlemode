public class AbilityCleave : AbilityProcableOnce, IApplicableAfterAttack
{
    public void Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters, AttackResult result)
    {
        if (WasUsed) return;

        if (result.Killed > 0)
        {
            WasUsed = true;
            owner.Attack(target, parameters.IsRanged, false, parameters.TriggerEvents);
            WasUsed = false;
        }
    }

    public override double OffensePotentialMultiplier => 2.0;
}
