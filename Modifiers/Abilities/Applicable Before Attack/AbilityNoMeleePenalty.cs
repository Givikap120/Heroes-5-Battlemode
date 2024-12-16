public class AbilityNoMeleePenalty : IAbility, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.AttackType == AttackType.MeleeWithPenalty)
        {
            parameters.AttackType = AttackType.Melee;
        }
            
        return parameters;
    }
}
