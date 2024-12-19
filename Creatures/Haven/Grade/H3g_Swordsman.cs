public class H3g_Swordsman : H3_Footman
{
    public H3g_Swordsman()
    {
        Name = "Swordsman";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Attack = 5;
        Stats.Defense = 9;

        Stats.MaxDamage = 5;
        Stats.HitPoints = 26;

        Stats.Power = 287;

        Abilities.Add(new AbilityShieldAllies());
    }
}

