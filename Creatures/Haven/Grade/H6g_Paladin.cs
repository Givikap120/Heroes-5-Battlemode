public class H6g_Paladin : H6_Cavalier
{
    public H6g_Paladin()
    {
        Name = "Paladin";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Attack = 24;
        Stats.Defense = 24;

        Stats.HitPoints = 100;
        Stats.Speed = 8;
        Stats.Initiative = 12;

        Stats.Power = 2520;

        //Abilities.Add(new AbilityLayHands());
        //Abilities.Add(new AbilityImmuneToFrenzy());
    }
}

