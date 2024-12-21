public class H6a_Champion : H6_Cavalier
{
    public H6a_Champion()
    {
        Name = "Champion";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 24;
        Stats.Defense = 20;
        Stats.MaxDamage = 35;

        Stats.HitPoints = 100;
        Stats.Speed = 8;
        Stats.Initiative = 12;

        Stats.Power = 2520;

        Abilities.Add(new AbilityChampionCharge());
    }
}

