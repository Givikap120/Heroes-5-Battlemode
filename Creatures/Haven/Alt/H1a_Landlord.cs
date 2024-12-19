public class H1a_Landlord : H1_Peasant
{
    public H1a_Landlord()
    {
        Name = "Landlord";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Attack = 2;
        Stats.MaxDamage = 2;
        Stats.HitPoints = 6;

        Stats.Power = 72;

        Abilities.Add(new AbilityAssault());
    }
}

