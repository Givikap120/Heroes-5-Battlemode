public abstract class AbilityApplicableAfterCreatureDeath : IBindableToInstance
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
                BattleHandler.Instance.CreatureDied += dead => Apply(value, dead);
            }
        }
    }

    public abstract void Apply(CreatureInstance owner, CreatureInstance dead);
}
