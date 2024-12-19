// This is empty ability that gives ability to shoot simply for the fact of existence in creature
public class AbilityShooter : Ability
{
    public const double NO_AMMO_OFFENSE_ESTIMATION_MULTIPLIER = 0.5;
    public const double OFFENSE_ESTIMATION_MULTIPLIER = 1.5;
    public override double OffensePotentialMultiplier => OFFENSE_ESTIMATION_MULTIPLIER;
}
