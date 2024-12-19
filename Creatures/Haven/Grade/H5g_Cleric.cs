public class H5g_Cleric : H5_Priest
{
    public H5g_Cleric()
    {
        Name = "Cleric";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Attack = 16;
        Stats.Defense = 16;

        Stats.HitPoints = 80;

        Stats.Power = 1487;
    }
}
