public class H2a_Crossbowman : H2_Archer
{
    public H2a_Crossbowman()
    {
        Name = "Crossbowman";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 5;
        Stats.Defense = 4;

        Stats.MaxDamage = 8;
        Stats.HitPoints = 10;
        Stats.Initiative = 8;

        Stats.Power = 203;

        Abilities.Add(new AbilityNoRangePenalty());
    }
}

