using Godot;

public interface IAttackable : IPlayfieldUnit
{
    public double Defense { get; }

    public void TakeDamage(double damage);
}
