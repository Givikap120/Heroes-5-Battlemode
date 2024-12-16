public class H3_Footman : Creature
{
    public H3_Footman()
    {
        Name = "Footman";
        IconPath = "res://Assets/Creatures/Heaven/Base/ico_Footman_128.dds";
        Tier = 3;
        Grade = GradeType.Base;

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
