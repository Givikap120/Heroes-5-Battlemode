public class AbilityAssault : AbilityProcableOnce, IChanceAbility, IApplicableAfterAttack
{
    public void Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters, AttackResult result)
    {
        if (WasUsed || parameters.IsCounterAttack) return;

        double targetHP = target.TotalHP;
        if (targetHP == 0) return;


        bool isSuccesful = IChanceAbility.TryTriggerProc(owner.TotalHP, targetHP, 1.0);
        if (!isSuccesful) return;

        WasUsed = true;
        owner.Attack(target, parameters.IsRanged, false, parameters.TriggerEvents);
        WasUsed = false;
    }

    public override double OffensePotentialMultiplier => 1.25;
}
