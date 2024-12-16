using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract class Creature
{
    public string Name = "Empty Name";
    public string IconPath = "";

    public int Tier;
	public GradeType Grade = GradeType.Base;

    public enum GradeType
    {
		Base,
		Grade,
		Altgrade
	}

	public CreatureStats Stats;

	public List<IAbility> Abilities = null!;

    public bool IsShooter => Abilities.OfType<AbilityShooter>().Any();

    // public List<Spell> Spells;

    public void BindToInstance(CreatureInstance instance)
    {
        foreach (var ability in Abilities.OfType<IBindableToInstance>())
        {
            ability.ParentInstance = instance;
        }
    }
}
