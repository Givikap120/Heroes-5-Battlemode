public struct AttackResult
{
    public ICanAttack Attacker;
    public IAttackable Target;
    public AttackParameters AttackParameters;

    public double DamageDealt;
    public int Killed;
}
