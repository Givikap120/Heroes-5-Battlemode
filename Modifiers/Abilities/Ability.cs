public abstract class Ability : IAbility
{
    public virtual double OffensePotentialMultiplier => 1.0;
    public virtual double DefensePotentialMultiplier => 1.0;
}