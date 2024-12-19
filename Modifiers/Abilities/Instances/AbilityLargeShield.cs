public class AbilityLargeShield : Ability, IApplicableToRecievedDamage
{
    public double Apply(double damage, AttackType attackType)
    {
        if (attackType.IsRangedShot()) damage *= 0.5;
        return damage;
    }

    public override double DefensePotentialMultiplier => 1.3;
}

