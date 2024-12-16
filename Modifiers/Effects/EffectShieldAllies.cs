public class EffectShieldAllies : Effect, IApplicableToRecievedDamage
{
    public double Apply(double damage, AttackType attackType)
    {
        if (attackType.IsRanged()) damage *= 0.5;
        return damage;
    }
}
