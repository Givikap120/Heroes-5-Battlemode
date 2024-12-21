using System;
using System.Collections.Generic;
using System.Linq;

public static class DynamicStateCalculator
{
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
            value += CalculateStackValue(stack);
        }

        return value / Player.MAX_ARMY_SIZE;
    }

    public static double CalculateStackValue(CreatureInstance stack)
    {
        // Get all attacking actions
        List<SimulationAction> actions = [];

        AIPlayer.AddAttackActions(actions, stack);
        AIPlayer.AddMoveAndAttackActions(actions, stack);

        foreach (SimulationAction action in actions)
            action.CalculateStateValue(useDynamic: false);

        var bestAction = actions.MaxBy(a => a.StateValue);

        double value = bestAction?.StateValue ?? StaticStateCalculator.CalculateStateValue(stack);
        double initiativeWeight = stack.Initiative / Unit.BASE_INITIATIVE;
        double atbWeight = 1 / (stack.GetRemainingTurnsToMove() + 1);

        return value * initiativeWeight * atbWeight;
    }
}
