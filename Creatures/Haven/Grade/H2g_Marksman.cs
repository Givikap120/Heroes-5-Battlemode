﻿public class H2g_Marksman : H2_Archer
{
    public H2g_Marksman()
    {
        Name = "Marksman";
        Faction = "Haven";
        Grade = GradeType.Grade;

        AutoSetIconPath();

        Stats.Defense = 4;

        Stats.MaxDamage = 8;
        Stats.HitPoints = 10;
        Stats.Initiative = 8;

        Stats.Power = 199;

        Abilities.Add(new AbilityPreciseShot());
    }
}

