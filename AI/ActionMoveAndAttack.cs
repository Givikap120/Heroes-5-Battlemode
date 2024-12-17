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

    public override void CalculateStateValue()
    {
        MovableAttacker.SavePosition();
        MovableAttacker.SaveState();
        Target.SaveState();

        MovableAttacker.MoveTo(Coords, triggerEvents: false);
        AIExtensions.AttackAverage(MovableAttacker, Target);
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit);

        MovableAttacker.LoadPosition(silent: true);
        MovableAttacker.LoadState(silent: true);
        Target.LoadState(silent: true);
    }

}
