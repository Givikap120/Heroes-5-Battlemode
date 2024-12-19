public class EffectDefense : OneTurnEffect
{
    public EffectDefense(CreatureInstance parentInstance) : base(parentInstance)
    {
        ParentInstance.CurrentStats.Defense *= 1.3;
    }

    public override void Cancel()
    {
        ParentInstance.CurrentStats.Defense /= 1.3;
        base.Cancel();
    }
}
