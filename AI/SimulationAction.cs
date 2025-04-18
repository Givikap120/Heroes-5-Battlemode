﻿public abstract class SimulationAction
{
    public IUnit CurrentUnit;

    public SimulationAction(IUnit currentUnit)
    {
        CurrentUnit = currentUnit;
    }

    public abstract void MakeMove();
    public abstract void CalculateStateValue(bool useDynamic = true);

    public double StateValue = double.NaN;
}
