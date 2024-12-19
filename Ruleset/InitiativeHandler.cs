using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class InitiativeHandler
{
    public readonly List<Unit> Units = [];

    public InitiativeHandler(BattleHandler parent)
    {
        parent.PlayerAdded += AddPlayer;
;    }

    public void AddPlayer(Player player)
    {
        foreach (Unit? unit in player.Army) 
        {
            if (unit != null) addUnit(unit);
        }

        addUnit(player.Hero);

        player.CreatureDied += handleUnitDead;
    }

    private void addUnit(Unit unit)
    {
        Debug.Assert(double.IsNaN(unit.ATB));

        unit.ATB = GD.RandRange(0, 0.25);
        Units.Add(unit);
    }

    private void handleUnitDead(Unit unit)
    {
        Units.RemoveAll(u => u == unit);
    }

    public Unit GetNextUnit() => Units[GetNextUnitIndex(Units.Count,
        i => Units[i].ATB,
        (i, value) => Units[i].ATB += value,
        i => Units[i].Initiative)];

    public static void EndTurn(Unit currentUnit, bool isWait = false) => currentUnit.ATB = isWait ? 0.5 : 0.0;

    /// <summary>
    /// Generic-styled ATB algorithm that finds the next Unit and moves ATB scale to it.
    /// Created to allow simulation outside of real Unit values.
    /// </summary>
    /// <param name="size">Amount of units to iterate over</param>
    /// <param name="getATB">Getting ATB value by index</param>
    /// <param name="addATB">Adding ATB value by index</param>
    /// <param name="getInitiative">Getting Initiative value by index</param>
    /// <returns>Index of the next Unit</returns>
    public static int GetNextUnitIndex(int size, Func<int, double> getATB, Action<int, double> addATB, Func<int, double> getInitiative)
    {
        double[] remainingTurns = GetRemainingTurns(size, getATB, getInitiative);
        int closestIndex = remainingTurns.MinIndex();

        MoveATBScale(remainingTurns[closestIndex], size, addATB, getInitiative);

        Debug.Assert(getATB(closestIndex) == 1.0);

        return closestIndex;
    }

    /// <summary>
    /// Geneic-styled function for converting ATB values into turns remained for Unit to move.
    /// </summary>
    /// <param name="size">Amount of units to iterate over</param>
    /// <param name="getATB">Getting ATB value by index</param>
    /// <param name="getInitiative">Getting Initiative value by index</param>
    /// <returns></returns>
    public static double[] GetRemainingTurns(int size, Func<int, double> getATB, Func<int, double> getInitiative)
    {
        double[] remainingTurns = new double[size];

        for (int i = 0; i < size; i++)
        {
            remainingTurns[i] = (1 - getATB(i)) * Unit.BASE_INITIATIVE / getInitiative(i);
        }

        return remainingTurns;
    }

    /// <summary>
    /// Generic-styled ATB moving function.
    /// </summary>
    /// <param name="turns">Amount of (normalized) turns to move ATB scale by</param>
    /// <param name="size">Amount of units to iterate over</param>
    /// <param name="addATB">Adding ATB value by index</param>
    /// <param name="getInitiative">Getting Initiative value by index</param>
    public static void MoveATBScale(double turns, int size, Action<int, double> addATB, Func<int, double> getInitiative)
    {
        for (int i = 0; i < size; i++)
        {
            double movedAmount = turns * getInitiative(i) / Unit.BASE_INITIATIVE;
            addATB(i, movedAmount);
        }
    }
}
