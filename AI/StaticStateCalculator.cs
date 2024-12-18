using System;

public static class StaticStateCalculator
{
    public static double CalculateStackValue(CreatureInstance stack)
    {
        double damagePotential = CalculateDamagePotential(stack);
        double effectiveHP = CalculateEffectiveHP(stack);

        double stackValue = Math.Pow(damagePotential * damagePotential * effectiveHP, 0.8);
        return stackValue;
    }

    public static double CalculateDamagePotential(CreatureInstance stack)
    {
        double attackMultiplier = 1 + 0.05 * stack.CurrentStats.Attack;
        double initiativeMultiplier = stack.CurrentStats.Initiative / 10;

        double abilitiesMultiplier = 1.0;
        foreach (var ability in stack.Creature.Abilities)
        {
            abilitiesMultiplier *= ability.OffensePotentialMultiplier;
        }

        if (stack.Creature.IsShooter && !stack.CanAttackRanged())
        {
            abilitiesMultiplier /= AbilityShooter.OFFENSE_ESTIMATION_MULTIPLIER;
            abilitiesMultiplier /= AbilityNoMeleePenalty.OFFENSE_ESTIMATION_MULTIPLIER;
        }

        double counterAttackFactor = ((IAttackable)stack).CanCounterattack() ? 1.1 : 1.0;

        return stack.AverageDamage * attackMultiplier * initiativeMultiplier * abilitiesMultiplier * counterAttackFactor * stack.Amount;
    }

    public static double CalculateEffectiveHP(CreatureInstance stack)
    {
        double hp = stack.TotalHP;
        double defenseMultiplier = 1 + 0.05 * stack.CurrentStats.Defense;

        double abilitiesMultiplier = 1.0;
        foreach (var ability in stack.Creature.Abilities)
            abilitiesMultiplier *= ability.DefensePotentialMultiplier;

        return hp * defenseMultiplier * abilitiesMultiplier;
    }
}
