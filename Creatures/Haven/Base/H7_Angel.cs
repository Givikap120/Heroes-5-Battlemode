public class H7_Angel : Creature
{
    public H7_Angel()
    {
        Name = "Angel";
        Faction = "Haven";
        Tier = 7;
        Grade = GradeType.Base;

        AutoSetIconPath();

        Stats = new CreatureStats
        {
            Attack = 27,
            Defense = 27,
            MinDamage = 45,
            MaxDamage = 45,
            HitPoints = 180,
            Speed = 6,
            Initiative = 11,
            Power = 4866
        };

        Abilities = 
        [
            new AbilityLargeCreature()
            // Flyer
        ];
    }
}
