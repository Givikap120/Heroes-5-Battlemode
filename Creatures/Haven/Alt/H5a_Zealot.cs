public class H5a_Zealot : H5_Priest
{
    public H5a_Zealot()
    {
        Name = "Zealot";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 20;
        Stats.Defense = 14;

        Stats.HitPoints = 80;

        Stats.Power = 1523;
    }
}
