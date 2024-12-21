public class H4a_BattleGriffin : H4_Griffin
{
    public H4a_BattleGriffin()
    {
        Name = "Battle Griffin";
        Faction = "Haven";
        Grade = GradeType.Altgrade;

        AutoSetIconPath();

        Stats.Defense = 12;

        Stats.MinDamage = 6;
        Stats.MaxDamage = 12;

        Stats.HitPoints = 52;
        Stats.Initiative = 10;

        Stats.Power = 697;

        Abilities.Add(new AbilityBattleFrenzy());
        //Abilities.Add(new AbilityRushDive());
    }
}

