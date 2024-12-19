/// <summary>
/// Effect that's activating at it's creation and can be deactivated by using Cancel().
/// </summary>
public abstract class CancelableEffect : Effect
{
    protected CancelableEffect(CreatureInstance parentInstance) : base(parentInstance)
    {
    }

    /// <summary>
    /// By default it's just removing it from effects.
    /// In overriden function base.Cancel() should be called on the end.
    /// </summary>
    public virtual void Cancel()
    {
        ParentInstance.Effects.Remove(this);
    }
}
