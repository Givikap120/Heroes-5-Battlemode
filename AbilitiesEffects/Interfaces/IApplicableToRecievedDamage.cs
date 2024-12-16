public interface IApplicableToRecievedDamage : IAbility
{
    public double Apply(double damage, AttackType attackType);
}
