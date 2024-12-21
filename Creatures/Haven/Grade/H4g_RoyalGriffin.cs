public class H4g_RoyalGriffin : H4_Griffin
{
    public H4g_RoyalGriffin()
    {
        Name = "Royal Griffin";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Attack = 9;
        Stats.Defense = 8;

        Stats.MaxDamage = 15;
        Stats.HitPoints = 35;

        Stats.Power = 615;

        //Abilities.Add(new AbilityBattleDive());
    }
}

