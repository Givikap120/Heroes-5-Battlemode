public class AbilityNoMeleePenalty : Ability, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.AttackType == AttackType.MeleeWithPenalty)
        {
            parameters.AttackType = AttackType.Melee;
        }
            
        return parameters;
    }

    public override double OffensePotentialMultiplier => 1.25;
}
