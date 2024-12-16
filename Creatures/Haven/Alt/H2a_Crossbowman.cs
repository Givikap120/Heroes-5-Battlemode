public class H2a_Crossbowman : H2_Archer
{
    public H2a_Crossbowman()
    {
        Name = "Crossbowman";
        IconPath = "res://Assets/Creatures/Heaven/Altgrade/Crossbowman.(Texture).dds";
        Grade = GradeType.Altgrade;

        Stats.Attack = 5;
        Stats.Defense = 4;

        Stats.MaxDamage = 8;
        Stats.HitPoints = 10;
        Stats.Initiative = 8;

        Abilities.Add(new AbilityAssault());
    }
}

