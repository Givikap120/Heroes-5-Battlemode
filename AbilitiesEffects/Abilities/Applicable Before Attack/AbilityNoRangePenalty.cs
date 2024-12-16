public class AbilityNoRangePenalty : IAbility, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.ShootType == ShootType.Weak)
        {
            parameters.ShootType = ShootType.Strong;
        }
            
        return parameters;
    }
}
