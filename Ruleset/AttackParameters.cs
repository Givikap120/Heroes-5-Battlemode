using static ICanAttack;

public struct AttackParameters
{
    public bool IsCounterAttack;
    public bool IsRanged;
    public bool WillCounterAttack;

    public double BaseDamage;
    public double Attack;
    public double Defense;

    public ShootType ShootType = ShootType.Melee;

    public AttackParameters()
    {
    }
}
