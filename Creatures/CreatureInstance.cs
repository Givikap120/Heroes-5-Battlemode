using Godot;
using System;
using static Playfield;

public class CreatureInstance : Unit, ICanAttackMove, IAttackable
{
    public readonly Creature Creature;
    public Bindable<int> AmountBindable;
    public Bindable<Vector2I> CoordsBindable = new(new Vector2I(-1, -1));
    public CreatureStats CurrentStats;

    public CreatureInstance(Player player, Creature creature, int amount = 1)
        : base(player)
	{
        Creature = creature;
		AmountBindable = new(amount);
		CurrentStats = creature.Stats;

        CreatureDead = creature => player.TriggerUnitDead(this);
    }

    public event Action<CreatureInstance> CreatureDead;

    public override DrawableCreatureInstance CreateDrawableRepresentation() => SceneFactory.CreateDrawableCreatureInstance(this);

    public override int DecideTileChange(int tileType)
    {
        if (tileType == (int)TileType.Affected)
            return (int)TileType.Select;

        return -1;
    }

    public override double Initiative => CurrentStats.Initiative;
    public Vector2I Coords { get => CoordsBindable.Value; set => CoordsBindable.Value = value; }
    public double Speed => CurrentStats.Speed;

    public bool CanAttackRanged => true;

    public double Defense => CurrentStats.Defense;

    public bool Attack(IAttackable attackable)
    {
        double baseDamage = GD.RandRange(CurrentStats.MinDamage, CurrentStats.MaxDamage);

        double attack = CurrentStats.Attack;
        double defense = attackable.Defense;

        double multiplier = attack >= defense ?
            (1 + 0.05 * (attack - defense)) :
            1.0 / (1 + 0.05 * (defense - attack));

        double damage = baseDamage * multiplier * AmountBindable.Value;

        // Apply various effects to the damage
        attackable.TakeDamage(damage);

        return true;
    }

    public void TakeDamage(double damage)
    {
        // Try to damage HP first
        double absorbedWithHP = Math.Min(damage, CurrentStats.HitPoints);

        damage -= absorbedWithHP;
        CurrentStats.HitPoints -= absorbedWithHP;

        // If there's still damage to do - it will kill creatures
        if (damage > 0)
        {
            int deaths = (int)(damage / Creature.Stats.HitPoints);

            AmountBindable.Value -= deaths;
            damage -= deaths * Creature.Stats.HitPoints;
        }

        // If current creature is dead - restore HP
        if (CurrentStats.HitPoints == 0)
        {
            AmountBindable.Value--;
            CurrentStats.HitPoints = Creature.Stats.HitPoints - damage;
        }

        if (AmountBindable.Value <= 0)
        {
            AmountBindable.Value = 0;
            CreatureDead.Invoke(this);
        }
    }
}
