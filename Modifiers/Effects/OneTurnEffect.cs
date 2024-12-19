public abstract class OneTurnEffect : CancelableEffect
{
    protected OneTurnEffect(CreatureInstance parentInstance) : base(parentInstance)
    {
        BattleHandler.Instance.NewTurnStarted += unit =>
        {
            if (unit == parentInstance) Cancel();
        };
    }
}
