using Godot;

public class ActionMove : SimulationAction
{
    public ICanMove Movable => (ICanMove)CurrentUnit;
    public Vector2I Coords;
    public ActionMove(ICanMove currentUnit, Vector2I coords) : base(currentUnit)
    {
        Coords = coords;
    }

    public override void MakeMove() => BattleHandler.Instance.EmptyCellAction(Coords);

    public override void CalculateStateValue(bool useDynamic = true)
    {
        var savedPosition = Movable.Coords;

        Movable.MoveTo(Coords, triggerEvents: false);
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit, useDynamic);

        Movable.CoordsBindable.SetSilent(savedPosition);
    }
}
