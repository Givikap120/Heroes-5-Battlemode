public class AbilityAssault : IChanceAbility, IApplicableAfterAttack
{
    public void Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.IsCounterAttack) return;

        double targetHP = target.TotalHP;
        if (targetHP == 0) return;


        bool isSuccesful = IChanceAbility.TryTriggerProc(owner.TotalHP, targetHP, 1.0);
        if (!isSuccesful) return;

        owner.Attack(target, parameters.IsRanged, false);
    }
}
