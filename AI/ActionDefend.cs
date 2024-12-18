public class ActionDefend : SimulationAction
{
    public ActionDefend(Unit currentUnit) : base(currentUnit)
    {
    }
    public override void MakeMove() => BattleHandler.Instance.DefendAction();

    public override void CalculateStateValue(bool useDynamic = true)
    {
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit, useDynamic);
    }
}
