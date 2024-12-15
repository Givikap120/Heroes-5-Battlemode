using System.Collections.Generic;

public class H1_Peasant : Creature
{
    public H1_Peasant()
    {
        Name = "Peasant";
        IconPath = "res://Assets/Creatures/Heaven/Base/ico_Peasant_128.dds";
        Tier = 1;
        Grade = GradeType.Base;

        Stats = new CreatureStats
        {
            Attack = 1,
            Defense = 1,
            MinDamage = 1,
            MaxDamage = 1,
            HitPoints = 3,
            Speed = 4,
            Initiative = 8
        };

        Abilities = [];
    }
}
