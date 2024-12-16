public class H2g_Marksman : H2_Archer
{
    public H2g_Marksman()
    {
        Name = "Crossbowman";
        IconPath = "res://Assets/Creatures/Heaven/Grade/ico_Marksman_128.dds";
        Grade = GradeType.Grade;

        Stats.Defense = 4;

        Stats.MaxDamage = 8;
        Stats.HitPoints = 10;
        Stats.Initiative = 8;

        Abilities.Add(new AbilityBash());
    }
}

