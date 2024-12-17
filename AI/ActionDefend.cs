public class ActionDefend : SimulationAction
{
    public ActionDefend(Unit currentUnit) : base(currentUnit)
    {
    }
    public override void MakeMove() => BattleHandler.Instance.DefendAction();

    public override void CalculateStateValue()
    {
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit);
    }
}
