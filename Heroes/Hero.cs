using System.Diagnostics;

public partial class Hero : Unit, ICanAttack
{
    public override string IconPath { get; set; } = "";
    public int Level;
    public HeroStats CurrentStats;

    private HeroStats baseStats;
    public HeroStats BaseStats { get => baseStats; set { baseStats = value; CalculateCurrentStats(); } }

    public Hero(Player player) : base(player)
    {
    }

    public override DrawableUnit CreateDrawableRepresentation() => SceneFactory.CreateDrawableUnit(this);

    public void CalculateCurrentStats()
    {
        baseStats.Mana = (int)(baseStats.Knowledge * 10);
        CurrentStats = baseStats;

        // Skills
        // Artifacts
        // Effects
    }

    /// <summary>
    /// Hero always has initiative of 10
    /// </summary>
    public override double Initiative => 10;

    public double MinDamage => double.NaN;

    public double MaxDamage => double.NaN;

    public double AverageDamage => double.NaN;

    public override UnitState SaveState()
    {
        return new UnitState();
    }

    public override void LoadState(UnitState savedState, bool silent = true)
    {
    }

    public bool CanAttackRanged() => true;

    public AttackType GetAttackType(IAttackable attackable) => AttackType.Hero;

    public AttackParameters CalculateParameters(IAttackable target, bool triggerEvents, bool isCounterattack = false, MoveResult? moveBeforeAttack = null)
    {
        Debug.Assert(target.Tier >= 1 && target.Tier <= 8);

        int tierIndex = target.Tier - 1;

        double[] minDamage = [ 2, 1.0, 0.8, 0.5, 0.3, 0.2, 0.1, 0.01]; // Damage for lvl 1
        double[] maxDamage = [12, 9.0, 6.5, 4.5, 3.0, 2.0, 1.5, 0.15]; // Damage for lvl 31

        double killsPerLevel = (maxDamage[tierIndex] - minDamage[tierIndex]) / 30;
        double kills = minDamage[tierIndex] + (Level - 1) * killsPerLevel;
        double damage = kills * target.MaxHP;

        return new AttackParameters
        {
            TriggerEvents = triggerEvents,
            BaseDamage = damage
        };
    }

    public double CalculateDamageFromParameters(AttackParameters parameters) => parameters.BaseDamage;

    public void AttackFromParameters(IAttackable target, AttackParameters parameters)
    {
        double damage = CalculateDamageFromParameters(parameters);
        target.TakeDamage(damage, parameters.AttackType, parameters.TriggerEvents);
    }

    public bool Attack(IAttackable target, bool triggerEvents, bool isCounterattack = false, MoveResult? moveBeforeAttack = null)
    {
        AttackParameters parameters = CalculateParameters(target, triggerEvents: triggerEvents);
        AttackFromParameters(target, parameters);
        return true;
    }
}
