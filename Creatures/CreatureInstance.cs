using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICanAttack;
using static Playfield;

public class CreatureInstance : Unit, ICanAttackMove, IAttackable
{
    public readonly Creature Creature;
    public Bindable<int> AmountBindable;
    public Bindable<Vector2I> CoordsBindable = new(new Vector2I(-1, -1));
    public CreatureStats CurrentStats;

    public int AttackedOnThisTurn { get; set; }

    public event Action<CreatureInstance> CreatureDead;

    public CreatureInstance(BattleHandler battleHandler, Player player, Creature creature, int amount = 1)
        : base(player)
	{
        Creature = creature;
		AmountBindable = new(amount);
		CurrentStats = creature.Stats;

        battleHandler.NewTurnStarted += handleTurnUpdate;

        CreatureDead = _ => 
        {
            battleHandler.NewTurnStarted -= handleTurnUpdate;
            player.TriggerUnitDead(this);
        };
    }
    public override DrawableCreatureInstance CreateDrawableRepresentation() => SceneFactory.CreateDrawableCreatureInstance(this);

    private void handleTurnUpdate(Unit? unit)
    {
        // For now - only handle if it's this creature turn
        if (unit != this) return;

        AttackedOnThisTurn = 0;
    }

    public Vector2I Coords { get => CoordsBindable.Value; set => CoordsBindable.Value = value; }
    public int Amount { get => AmountBindable.Value; set => AmountBindable.Value = value; }

    public override double Initiative => CurrentStats.Initiative;
    public double Speed => CurrentStats.Speed;

    public double Defense => CurrentStats.Defense;

    public double TotalHP => CurrentStats.HitPoints + (Amount - 1) * Creature.Stats.HitPoints;

    public override int DecideTileChange(int tileType)
    {
        if (tileType == (int)TileType.Affected || tileType == (int)TileType.Aimable)
            return (int)TileType.Select;

        return -1;
    }

    public bool CanAttackRanged(IEnumerable<IPlayfieldUnit> units)
    {
        if (!Creature.IsShooter || CurrentStats.Shots == 0)
            return false;

        bool isBlocked = false;
        foreach (var unit in units)
        {
            if (!unit.IsAlly(this) && unit.IsNeighboring(this))
            {
                isBlocked = true;
                break;
            }
        }

        return !isBlocked;
    }

    public bool Attack(IAttackable target, bool allowRanged, bool isCounterattack)
    {
        if (Amount <= 0) return false;

        double baseDamage = GD.RandRange(CurrentStats.MinDamage, CurrentStats.MaxDamage);

        double attack = CurrentStats.Attack;
        double defense = target.Defense;

        double armorMultiplier = attack >= defense ?
            (1 + 0.05 * (attack - defense)) :
            1.0 / (1 + 0.05 * (defense - attack));

        double shootingMultiplier = 1.0;
        bool isRanged = false;

        if (Creature.IsShooter)
        {
            ShootType shootType = CanShootTarget(target);

            if ((shootType == ShootType.None) || (!allowRanged && shootType != ShootType.Melee))
                return false;

            if (shootType != ShootType.Melee)
                isRanged = true;

            shootingMultiplier = shootType switch
            {
                ShootType.Melee => 0.5,
                ShootType.Weak => 0.5,
                ShootType.Strong => 1.0,
                _ => 0.0
            };
        }

        double damage = baseDamage * armorMultiplier * shootingMultiplier * Amount;

        bool willCounterAttack = !isCounterattack && target.WillCounterattack(this);

        foreach (var ability in Creature.Abilities.OfType<IApplicableBeforeAttack>())
        {
            willCounterAttack &= ability.Apply(this, target, isRanged, isCounterattack);
        }

        // Apply various effects to the damage
        target.TakeDamage(damage);

        if (willCounterAttack && target is ICanAttack counterAttacker)
            counterAttacker.Attack(this, isCounterattack: true);

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

            Amount -= deaths;
            damage -= deaths * Creature.Stats.HitPoints;
        }

        // If current creature is dead - restore HP
        if (CurrentStats.HitPoints == 0)
        {
            Amount--;
            CurrentStats.HitPoints = Creature.Stats.HitPoints - damage;
        }

        if (Amount <= 0)
        {
            Amount = 0;
            CreatureDead.Invoke(this);
        }

        AttackedOnThisTurn++;
    }

    public ShootType CanShootTarget(IAttackable attackable)
    {
        if (!Creature.IsShooter || CurrentStats.Shots == 0)
            return ShootType.None;

        // Don't attack allies
        if (this.IsAlly(attackable))
            return ShootType.None;

        double distanceToTarget = (Coords - attackable.Coords).Length();

        // If it's an enemy - it should be at least 1 tile away
        if (distanceToTarget < 2)
            return ShootType.Melee;

        ShootType result = distanceToTarget <= 6 ? ShootType.Strong : ShootType.Weak;

        // IApplicableToShootType.Apply

        return result;
    }
}
