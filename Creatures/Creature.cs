using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract class Creature
{
    public string Name = "Empty Name";
    public string Faction = "Empty Faction";
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

    public bool HasAbility<T>() => Abilities.OfType<T>().Any();

    public bool IsShooter => HasAbility<AbilityShooter>();

    // public List<Spell> Spells;

    public void BindToInstance(CreatureInstance instance)
    {
        foreach (var ability in Abilities.OfType<IBindableToInstance>())
        {
            ability.ParentInstance = instance;
        }
    }

    protected void AutoSetIconPath()
    {
        string grade = Grade.ToString();
        string faction = Faction.Replace(" ", "");
        string name = Name.Replace(" ", "");

        IconPath = Grade == GradeType.Altgrade
            ? $"res://Assets/Creatures/{faction}/{grade}/{name}.(Texture).dds"
            : $"res://Assets/Creatures/{faction}/{grade}/ico_{name}_128.dds";
    }
}
