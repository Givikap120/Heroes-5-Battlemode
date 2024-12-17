using System;

public static class AIExtensions
{
    public static void AttackAverage(ICanAttack attacker, IAttackable target)
    {
        var parameters = attacker.CalculateParameters(target);

        if (attacker is IHasRandomDamage randomAttacker)
            parameters.BaseDamage = randomAttacker.AverageDamage;

        attacker.AttackFromParameters(target, parameters, triggerEvents: false);
    }

    public static double CalculateStateValue(IUnit unit)
    {
        double allyStateValue = CalculatePlayerStateValue(unit.Player);
        double enemyStateValue = CalculatePlayerStateValue(BattleHandler.Instance.GetEnemyPlayer(unit.Player)!);
        double stateValue = allyStateValue - enemyStateValue;
        return stateValue;
    }

    public static double CalculatePlayerStateValue(Player player)
    {
        double value = 0;

        foreach (var stack in player.AliveArmy)
        {
            double damagePotential = CalculateDamagePotential(stack);
            double effectiveHP = CalculateEffectiveHP(stack);
            double stackValue = Math.Pow(damagePotential * damagePotential * effectiveHP, 0.8);
            value += stackValue;
        }

        return Math.Log10(value);
    }

    public static double CalculateDamagePotential(CreatureInstance stack)
    {
        double attackMultiplier = 1 + 0.05 * stack.CurrentStats.Attack;
        double initiativeMultiplier = stack.CurrentStats.Initiative / 10;

        double abilitiesMultiplier = 1.0;
        // For each ability .DamagePotentialMultiplier

        return stack.AverageDamage * attackMultiplier * initiativeMultiplier * abilitiesMultiplier * stack.Amount;
    }

    public static double CalculateEffectiveHP(CreatureInstance stack)
    {
        double hp = stack.TotalHP;
        double defenseMultiplier = 1 + 0.05 * stack.CurrentStats.Defense;

        double abilitiesMultiplier = 1.0;
        // For each ability .DefensePotentialMultiplier

        return hp * defenseMultiplier * abilitiesMultiplier;
    }
}
