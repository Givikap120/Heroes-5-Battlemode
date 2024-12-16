public class H5g_Cleric : H5_Priest
{
    public H5g_Cleric()
    {
        Name = "Cleric";
        IconPath = "res://Assets/Creatures/Heaven/Grade/ico_Cleric_128.dds";
        Grade = GradeType.Grade;

        Stats.Attack = 16;
        Stats.Defense = 16;

        Stats.HitPoints = 80;

        Stats.Power = 1487;
    }
}
