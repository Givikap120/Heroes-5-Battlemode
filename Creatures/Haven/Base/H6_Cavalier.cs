public class H6_Cavalier : Creature
{
    public H6_Cavalier()
    {
        Name = "Cavalier";
        Faction = "Haven";
        Tier = 6;
        Grade = GradeType.Base;

        AutoSetIconPath();

        Stats = new CreatureStats
        {
            Attack = 23,
            Defense = 21,
            MinDamage = 20,
            MaxDamage = 30,
            HitPoints = 90,
            Speed = 7,
            Initiative = 11,
            Power = 2185
        };

        Abilities = 
        [
            new AbilityLargeCreature(),
            new AbilityJousting()
        ];
    }
}
