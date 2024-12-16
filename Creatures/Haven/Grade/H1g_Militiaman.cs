public class H1g_Militiaman : H1_Peasant
{
    public H1g_Militiaman()
    {
        Name = "Militiaman";
        IconPath = "res://Assets/Creatures/Heaven/Grade/ico_Militiaman_128.dds";
        Grade = GradeType.Grade;

        Stats.Defense = 2;
        Stats.MaxDamage = 2;
        Stats.HitPoints = 6;

        Stats.Power = 72;

        Abilities.Add(new AbilityBash());
    }
}

