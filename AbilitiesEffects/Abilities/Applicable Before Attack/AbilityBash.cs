public class AbilityBash : IChanceAbility, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.IsCounterAttack) return parameters;

        bool isSuccesful = IChanceAbility.TryTriggerProc(owner.TotalHP, target.TotalHP, 1.5);
        if (!isSuccesful) return parameters;

        target.ATB = 0;
        parameters.WillCounterAttack = false;
        return parameters;
    }
}
