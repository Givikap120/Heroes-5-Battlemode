using System;
using System.Collections.Generic;
using System.Linq;

public static class DynamicStateCalculator
{
    public static double CalculateStackValue(CreatureInstance stack)
    {
        // Get all attacking actions
        List<SimulationAction> actions = [];

        AIPlayer.AddAttackActions(actions, stack);
        AIPlayer.AddMoveAndAttackActions(actions, stack);

        foreach (SimulationAction action in actions)
            action.CalculateStateValue(useDynamic: false);

        var bestAction = actions.MaxBy(a => a.StateValue);

        double value = bestAction?.StateValue ?? AIExtensions.CalculateStateValue(stack, useDynamic: false);
        double initiativeWeight = stack.Initiative / Unit.BASE_INITIATIVE;
        double atbWeight = 1 / (stack.GetRemainingTurnsToMove() + 1);

        return value * initiativeWeight * atbWeight;
    }
}
