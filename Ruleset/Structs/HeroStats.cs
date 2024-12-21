public struct HeroStats
{
    public double Attack;
    public double Defense;
    public double Spellpower;
    public double Knowledge;

    public double Morale;
    public double Luck;

    public int Mana;
}

public static class HeroStatsExtensions
{
    public static CreatureStats ApplyToCreatureStats(this HeroStats heroStats, CreatureStats creatureStats)
    {
        creatureStats.Attack += heroStats.Attack;
        creatureStats.Defense += heroStats.Defense;
        return creatureStats;
    }
}