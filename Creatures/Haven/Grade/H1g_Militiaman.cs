public class H1g_Militiaman : H1_Peasant
{
    public H1g_Militiaman()
    {
        Name = "Militiaman";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Defense = 2;
        Stats.MaxDamage = 2;
        Stats.HitPoints = 6;

        Stats.Power = 72;

        Abilities.Add(new AbilityBash());
    }
}

