public class EffectShieldAllies : Effect, IApplicableToRecievedDamage
{
    public EffectShieldAllies(CreatureInstance parentInstance) : base(parentInstance)
    {
    }

    public double Apply(double damage, AttackType attackType)
    {
        if (attackType.IsRangedShot()) damage *= 0.5;
        return damage;
    }
}
