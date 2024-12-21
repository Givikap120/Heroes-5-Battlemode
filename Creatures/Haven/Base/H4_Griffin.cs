public class H4_Griffin : Creature
{
    public H4_Griffin()
    {
        Name = "Griffin";
        Faction = "Haven";
        Tier = 4;
        Grade = GradeType.Base;

        AutoSetIconPath();

        Stats = new CreatureStats
        {
            Attack = 7,
            Defense = 5,
            MinDamage = 5,
            MaxDamage = 10,
            HitPoints = 30,
            Speed = 7,
            Initiative = 15,
            Power = 524
        };

        Abilities = 
        [
            new AbilityLargeCreature()
            // Flyer
            // Unlimited Retaliation
            // Immune to Blind
        ];
    }
}
