public class AbilityAssault : IChanceAbility, IApplicableAfterAttack
{
    public void Apply(CreatureInstance owner, IAttackable target, bool isRanged, bool isCounterAttack)
    {
        if (isCounterAttack) return;

        double targetHP = target.TotalHP;
        if (targetHP == 0) return;


        bool isSuccesful = IChanceAbility.TryTriggerProc(owner.TotalHP, targetHP, 1.0);
        if (!isSuccesful) return;

        owner.Attack(target, isRanged, false);
    }
}
