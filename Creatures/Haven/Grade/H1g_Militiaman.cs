using System.Collections.Generic;

public class H1g_Militiaman : H1_Peasant
{
    public H1g_Militiaman()
    {
        Name = "Militiaman";
        IconPath = "res://Assets/Creatures/Heaven/Grade/ico_Militiaman_128.dds";
        Tier = 1;
        Grade = GradeType.Grade;

        Stats.Defense = 2;
        Stats.MaxDamage = 2;
        Stats.HitPoints = 6;

        Abilities.Add(new AbilityBash());
    }
}

