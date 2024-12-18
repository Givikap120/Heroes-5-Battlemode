public abstract class AbilityApplicableAfterTurnBegin : Ability, IBindableToInstance
{
    private CreatureInstance? parentInstance;
    public CreatureInstance? ParentInstance
    {
        get => parentInstance;
        set
        {
            if (value != null && ParentInstance == null)
            {
                parentInstance = value;
                BattleHandler.Instance.NewTurnStarted += _ => Apply(value);
            }
        }
    }

    public abstract void Apply(CreatureInstance owner);
}
