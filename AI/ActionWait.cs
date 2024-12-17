public class ActionWait : SimulationAction
{
    public ActionWait(Unit currentUnit) : base(currentUnit)
    {
    }

    public override void MakeMove() => BattleHandler.Instance.WaitAction();

    public override void CalculateStateValue()
    {
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit);
    }
}
