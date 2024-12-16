public class H5_Priest : Creature
{
    public H5_Priest()
    {
        Name = "Priest";
        IconPath = "res://Assets/Creatures/Heaven/Base/ico_Priest_128.dds";
        Tier = 5;
        Grade = GradeType.Base;

        Stats = new CreatureStats
        {
            Attack = 12,
            Defense = 12,
            MinDamage = 9,
            MaxDamage = 12,
            HitPoints = 54,
            Speed = 5,
            Initiative = 10,
            Shots = 7,
            Power = 1086
        };

        Abilities =
        [
            new AbilityShooter(),
            new AbilityNoMeleePenalty()
        ];
    }
}
