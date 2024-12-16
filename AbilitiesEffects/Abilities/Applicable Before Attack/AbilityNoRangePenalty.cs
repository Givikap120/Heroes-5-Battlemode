public class AbilityNoRangePenalty : IAbility, IApplicableBeforeAttack
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters)
    {
        if (parameters.ShootType == ICanAttack.ShootType.Weak)
        {
            parameters.ShootType = ICanAttack.ShootType.Strong;
        }
            
        return parameters;
    }
}
