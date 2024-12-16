/// <summary>
/// Ability that requires binding to CreatureInstance
/// Use abstract class for category
/// </summary>
public interface IBindableToInstance : IAbility
{
    public CreatureInstance? ParentInstance { get; set; }
}
