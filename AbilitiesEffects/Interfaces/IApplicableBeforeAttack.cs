public interface IApplicableBeforeAttack : IAbility
{
    public AttackParameters Apply(CreatureInstance owner, IAttackable target, AttackParameters parameters);
}