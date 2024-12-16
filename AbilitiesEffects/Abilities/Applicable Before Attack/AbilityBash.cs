public class AbilityBash : IChanceAbility, IApplicableBeforeAttack
{
    public bool Apply(CreatureInstance owner, IAttackable target, bool isRanged, bool isCounterAttack)
    {
        if (isCounterAttack) return true;

        bool isSuccesful = IChanceAbility.TryTriggerProc(owner.TotalHP, target.TotalHP, 1.5);
        if (!isSuccesful) return true;

        target.ATB = 0;
        return false;
    }
}
