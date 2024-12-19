public class H3_Footman : Creature
{
    public H3_Footman()
    {
        Name = "Footman";
        Faction = "Haven";
        Tier = 3;
        Grade = GradeType.Base;

        AutoSetIconPath();

        Stats = new CreatureStats
        {
            Attack = 4,
            Defense = 8,
            MinDamage = 2,
            MaxDamage = 4,
            HitPoints = 16,
            Speed = 4,
            Initiative = 8,
            Power = 201
        };

        Abilities = 
        [
            new AbilityEnraged(),
            new AbilityLargeShield(),
            new AbilityBash()
        ];
    }
}
