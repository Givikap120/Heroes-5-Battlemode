public class H3g_Swordsman : H3_Footman
{
    public H3g_Swordsman()
    {
        Name = "Swordsman";
        IconPath = "res://Assets/Creatures/Heaven/Grade/ico_Swordsman_128.dds";
        Grade = GradeType.Grade;

        Stats.Attack = 5;
        Stats.Defense = 9;

        Stats.MaxDamage = 5;
        Stats.HitPoints = 26;

        //Abilities.Add(ShieldAllies);
    }
}

