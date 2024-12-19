public class H3a_Vindicator : H3_Footman
{
    public H3a_Vindicator()
    {
        Name = "Vindicator";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 8;

        Stats.MaxDamage = 5;
        Stats.HitPoints = 26;

        Stats.Power = 299;

        Abilities.RemoveAll(ability => ability.GetType() == typeof(AbilityBash));
        Abilities.Add(new AbilityCleave());
    }
}

