using Godot;
using System.Collections.Generic;

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

	public List<Ability> Abilities = null!;

	// public List<Spell> Spells;
}
