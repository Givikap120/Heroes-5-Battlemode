public interface IApplicableAfterAttack : IAbility
{
    public void Apply(CreatureInstance owner, IAttackable target, bool isRanged, bool isCounterAttack);
}