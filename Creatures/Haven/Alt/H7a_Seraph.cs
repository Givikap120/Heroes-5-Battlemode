public class H7a_Seraph : H7_Angel
{
    public H7a_Seraph()
    {
        Name = "Seraph";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 35;
        Stats.Defense = 25;
        Stats.MaxDamage = 25;
        Stats.MaxDamage = 75;

        Stats.HitPoints = 220;
        Stats.Initiative = 8;

        Stats.Power = 6003;

        //Abilities.Add(new AbilityCaster());
    }
}

