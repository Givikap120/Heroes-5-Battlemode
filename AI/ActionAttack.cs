public class ActionAttack : SimulationAction
{
    public ICanAttack Attacker => (ICanAttack)CurrentUnit;
    public IAttackable Target;

    public ActionAttack(ICanAttack currentUnit, IAttackable target) : base(currentUnit)
    {
        Target = target;
    }
    public override void MakeMove() => BattleHandler.Instance.UnitAction(Target);

    public override void CalculateStateValue(bool useDynamic = true)
    {
        var attackerState = Attacker.SaveState();
        var targetState = Target.SaveState();

        AIExtensions.AttackAverage(Attacker, Target);
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit, useDynamic);

        Attacker.LoadState(attackerState);
        Target.LoadState(targetState);
    }
}
