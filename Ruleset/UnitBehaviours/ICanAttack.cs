public interface ICanAttack
{
    public bool CanAttackRanged { get; }

    public bool Attack(IAttackable attackable);
}
