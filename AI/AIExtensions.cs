using System;

public static class AIExtensions
{
    public static double CalculateStateValue(IUnit unit, bool useDynamic = true)
    {
        double allyStateValue = CalculatePlayerStateValue(unit.Player, useDynamic);
        double enemyStateValue = CalculatePlayerStateValue(BattleHandler.Instance.GetEnemyPlayer(unit.Player)!, useDynamic);
        double stateValue = allyStateValue - enemyStateValue;
        return stateValue;
    }

    public static double CalculatePlayerStateValue(Player player, bool useDynamic = true)
    {
        double value = 0;

        foreach (var stack in player.AliveArmy)
        {
            value += useDynamic ? DynamicStateCalculator.CalculateStackValue(stack) : StaticStateCalculator.CalculateStackValue(stack);
        }

        return useDynamic ? value : Math.Log10(value);
    }

    public static void AttackAverage(ICanAttack attacker, IAttackable target)
    {
        var parameters = attacker.CalculateParameters(target);

        if (attacker is IHasRandomDamage randomAttacker)
            parameters.BaseDamage = randomAttacker.AverageDamage;

        attacker.AttackFromParameters(target, parameters, triggerEvents: false);
    }
}
