public class ActionWait : SimulationAction
{
    public ActionWait(Unit currentUnit) : base(currentUnit)
    {
    }

    public override void MakeMove() => BattleHandler.Instance.WaitAction();

    public override void CalculateStateValue(bool useDynamic = true)
    {
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit, useDynamic);
    }
}
