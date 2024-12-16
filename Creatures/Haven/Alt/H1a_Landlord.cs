public class H1a_Landlord : H1_Peasant
{
    public H1a_Landlord()
    {
        Name = "Landlord";
        IconPath = "res://Assets/Creatures/Heaven/Altgrade/Landlord.(Texture).dds";
        Grade = GradeType.Altgrade;

        Stats.Attack = 2;
        Stats.MaxDamage = 2;
        Stats.HitPoints = 6;

        Abilities.Add(new AbilityAssault());
    }
}

