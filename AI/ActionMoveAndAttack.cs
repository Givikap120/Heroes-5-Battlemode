using Godot;

public class ActionMoveAndAttack : SimulationAction
{
    public ICanMoveAttack MovableAttacker => (ICanMoveAttack)CurrentUnit;
    public Vector2I Coords;
    public IAttackable Target;

    public ActionMoveAndAttack(ICanMoveAttack currentUnit, Vector2I coords, IAttackable target) : base(currentUnit)
    {
        Coords = coords;
        Target = target;
    }

    public override void MakeMove() => BattleHandler.Instance.UnitWithCellAction(Target, Coords);

    public override void CalculateStateValue(bool useDynamic = true)
    {
        var attackerState = MovableAttacker.SaveState();
        var targetState = Target.SaveState();

        MovableAttacker.MoveTo(Coords, triggerEvents: false);
        AIExtensions.AttackAverage(MovableAttacker, Target);
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit, useDynamic);

        MovableAttacker.LoadState(attackerState);
        Target.LoadState(targetState);
    }

}
