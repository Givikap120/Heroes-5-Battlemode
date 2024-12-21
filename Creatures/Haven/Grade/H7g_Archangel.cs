public class H7g_Archangel : H7_Angel
{
    public H7g_Archangel()
    {
        Name = "Archangel";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Attack = 31;
        Stats.Defense = 31;
        Stats.MaxDamage = 50;
        Stats.MaxDamage = 50;

        Stats.HitPoints = 220;
        Stats.Initiative = 8;

        Stats.Power = 6153;

        //Abilities.Add(new AbilityResurrectAllies());
    }
}

