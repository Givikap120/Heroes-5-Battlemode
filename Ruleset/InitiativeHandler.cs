using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class InitiativeHandler
{
    public const double BASE_INITIATIVE = 10;

    public readonly List<Unit> List = [];

    public void AddPlayer(Player player)
    {
        foreach (Unit? unit in player.Army) 
        { 
            if (unit == null) continue;

            Debug.Assert(double.IsNaN(unit.ATB));

            unit.ATB = GD.RandRange(0, 0.25);
            List.Add(unit);
        }

        player.CreatureDead += handleUnitDead;
    }

    private void handleUnitDead(Unit unit)
    {
        List.RemoveAll(u => u == unit);
    }

    public Unit GetNextUnit()
    {
        var remainingTurns = List.Select(u => (1 - u.ATB) * BASE_INITIATIVE / u.Initiative).ToArray();
        int closestIndex = remainingTurns.MinIndex();

        moveATBScale(remainingTurns[closestIndex]);

        Debug.Assert(List[closestIndex].ATB == 1.0);

        return List[closestIndex];
    }

    private void moveATBScale(double turns)
    {
        foreach (var unit in List)
        {
            double movedAmount = turns * unit.Initiative / BASE_INITIATIVE;
            unit.ATB += movedAmount;
        }
    }
}
