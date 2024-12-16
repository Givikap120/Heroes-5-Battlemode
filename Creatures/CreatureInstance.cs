using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public event Action<CreatureInstance> CreatureDead = delegate { };

    public CreatureInstance(BattleHandler? battleHandler, Player player, Creature creature, int amount = 1)
        : base(player)
	{
        Creature = creature;
		AmountBindable = new(amount);
		CurrentStats = creature.Stats;

        creature.BindToInstance(this);

        if (battleHandler != null) 
        {
            battleHandler.NewTurnStarted += handleTurnUpdate;

            CreatureDead = _ =>
            {
                battleHandler.NewTurnStarted -= handleTurnUpdate;
                player.TriggerUnitDead(this);
            };
        }
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

    public double TotalHP => Amount > 0 ? CurrentStats.HitPoints + (Amount - 1) * Creature.Stats.HitPoints : 0;

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

    private AttackParameters calculateParameters(IAttackable target, bool allowRanged, bool isCounterattack)
    {
        var parameters = new AttackParameters
        {
            IsCounterAttack = isCounterattack,
            BaseDamage = GD.RandRange(CurrentStats.MinDamage, CurrentStats.MaxDamage),
            WillCounterAttack = !isCounterattack && target.WillCounterattack(this),

            Attack = CurrentStats.Attack,
            Defense = target.Defense
        };

        if (Creature.IsShooter)
        {
            parameters.AttackType = CanShootTarget(target);

            if (!allowRanged && parameters.AttackType != AttackType.None)
                parameters.AttackType = AttackType.MeleeWithPenalty;

            parameters.IsRanged = parameters.AttackType.IsRanged();
        }

        return parameters;
    }

    public bool Attack(IAttackable target, bool allowRanged, bool isCounterattack)
    {
        if (Amount <= 0) return false;

        AttackParameters parameters = calculateParameters(target, allowRanged, isCounterattack);

        if (parameters.AttackType == AttackType.None)
            return false;

        // Effects before attack
        foreach (var ability in Creature.Abilities.OfType<IApplicableBeforeAttack>())
            parameters = ability.Apply(this, target, parameters);


        // Calculating attack with parameters
        double armorMultiplier = parameters.Attack >= parameters.Defense ?
            (1 + 0.05 * (parameters.Attack - parameters.Defense)) :
            1.0 / (1 + 0.05 * (parameters.Defense - parameters.Attack));

        double damage = parameters.BaseDamage * armorMultiplier * parameters.AttackType.GetMultiplier() * Amount;

        // Attack
        var attackResult = target.TakeDamage(damage, parameters.AttackType);

        // Counterattack
        if (parameters.WillCounterAttack && target is ICanAttack counterAttacker)
            counterAttacker.Attack(this, isCounterattack: true);

        // Effects after attack
        foreach (var ability in Creature.Abilities.OfType<IApplicableAfterAttack>())
            ability.Apply(this, target, parameters, attackResult);

        return true;
    }

    public AttackResult TakeDamage(double damage, AttackType attackType)
    {
        // Effects on damage
        foreach (var ability in Creature.Abilities.OfType<IApplicableToRecievedDamage>())
            damage = ability.Apply(damage, attackType);

        AttackResult result = new AttackResult();

        double incomingDamage = damage;

        // Try to damage HP first
        double absorbedWithHP = Math.Min(damage, CurrentStats.HitPoints);

        damage -= absorbedWithHP;
        CurrentStats.HitPoints -= absorbedWithHP;

        // If there's still damage to do - it will kill creatures
        if (damage > 0)
        {
            result.Killed = (int)(damage / Creature.Stats.HitPoints);
            result.Killed = Math.Min(result.Killed, Amount);

            Amount -= result.Killed;
            damage -= result.Killed * Creature.Stats.HitPoints;
        }

        // If current creature is dead - restore HP
        if (CurrentStats.HitPoints == 0 && Amount > 0)
        {
            Amount--;
            CurrentStats.HitPoints = Creature.Stats.HitPoints - damage;
            damage = 0;
        }

        Debug.Assert(Amount >= 0);
        result.DamageDealt = incomingDamage - damage;

        if (Amount == 0)
        {
            CreatureDead.Invoke(this);
        }

        AttackedOnThisTurn++;

        return result;
    }

    public AttackType CanShootTarget(IAttackable attackable)
    {
        // Don't attack allies
        if (this.IsAlly(attackable))
            return AttackType.None;

        double distanceToTarget = attackable.DistanceTo(this);

        // If it's an enemy - it should be at least 1 tile away
        if (distanceToTarget < 2)
            return Creature.IsShooter ? AttackType.MeleeWithPenalty : AttackType.Melee;

        if (CurrentStats.Shots == 0)
            return AttackType.None;

        AttackType result = distanceToTarget <= 6 ? AttackType.RangedStrong : AttackType.RangedWeak;

        return result;
    }
}
