public class ActionAttack : SimulationAction
{
    public ICanAttack Attacker => (ICanAttack)CurrentUnit;
    public IAttackable Target;

    public ActionAttack(ICanAttack currentUnit, IAttackable target) : base(currentUnit)
    {
        Target = target;
    }
    public override void MakeMove() => BattleHandler.Instance.UnitAction(Target);

    public override void CalculateStateValue()
    {
        Attacker.SaveState();
        Target.SaveState();

        AIExtensions.AttackAverage(Attacker, Target);
        StateValue = AIExtensions.CalculateStateValue(CurrentUnit);

        Attacker.LoadState(silent: true);
        Target.LoadState(silent: true);
    }
}
