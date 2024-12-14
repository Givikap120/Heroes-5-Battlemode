using System;

public interface IAttackable
{
    public double Defense { get; }

    public void TakeDamage(double damage);
}
