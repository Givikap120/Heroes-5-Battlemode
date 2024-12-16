public class H2_Archer : Creature
{
    public H2_Archer()
    {
        Name = "Archer";
        IconPath = "res://Assets/Creatures/Heaven/Base/ico_Archer_128.dds";
        Tier = 2;
        Grade = GradeType.Base;

        Stats = new CreatureStats
        {
            Attack = 4,
            Defense = 3,
            MinDamage = 2,
            MaxDamage = 4,
            HitPoints = 7,
            Speed = 4,
            Initiative = 9,
            Shots = 10,
            Power = 140
        };

        Abilities = 
        [
            new AbilityShooter()
        ];
    }
}
