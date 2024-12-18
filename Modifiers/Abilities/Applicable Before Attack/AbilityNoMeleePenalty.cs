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

    public const double OFFENSE_ESTIMATION_MULTIPLIER = 1.25;

    public override double OffensePotentialMultiplier => OFFENSE_ESTIMATION_MULTIPLIER;
}
