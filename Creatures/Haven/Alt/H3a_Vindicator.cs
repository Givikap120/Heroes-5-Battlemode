public class H3a_Vindicator : H3_Footman
{
    public H3a_Vindicator()
    {
        Name = "Vindicator";
        IconPath = "res://Assets/Creatures/Heaven/Altgrade/Vindicator.(Texture).dds";
        Grade = GradeType.Altgrade;

        Stats.Attack = 8;

        Stats.MaxDamage = 5;
        Stats.HitPoints = 26;

        Abilities.RemoveAll(ability => ability.GetType() == typeof(AbilityBash));
        //Abilities.Add(Cleave);
    }
}

