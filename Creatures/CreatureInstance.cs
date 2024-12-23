using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class CreatureInstance : Unit, ICanMoveAttack, IAttackable, IHasRandomDamage
{
    public readonly Creature Creature;
    public CreatureStats CurrentStats;
    public Bindable<int> AmountBindable { get; set; }
    public Bindable<Vector2I> CoordsBindable { get; set; } = new(new Vector2I(-1, -1));

    public List<Effect> Effects = [];

    public bool HasEffect<T>() => Effects.OfType<T>().Any();

    public IEnumerable<T> ModifiersOfType<T>()
    {
        IEnumerable<T> abilities = Creature.Abilities.OfType<T>();
        IEnumerable<T> effects = Effects.OfType<T>();
        return abilities.Concat(effects);
    }

    public int AttackedOnThisTurn { get; set; }
    
    public event Action<CreatureInstance> CreatureDead = delegate { };

    public CreatureInstance(BattleHandler? battleHandler, Player player, Creature creature, int amount = 1)
        : base(player)
	{
        Creature = creature;
		AmountBindable = new(amount);

		CurrentStats = creature.Stats;
        CurrentStats = player.Hero.CurrentStats.ApplyToCreatureStats(CurrentStats);

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
    public override DrawableUnit CreateDrawableRepresentation() => SceneFactory.CreateDrawableCreatureInstance(this);

    private void handleTurnUpdate(Unit? unit)
    {
        // For now - only handle if it's this creature turn
        if (unit != this) return;

        AttackedOnThisTurn = 0;
    }

    public override string IconPath { get => Creature.IconPath; set => Creature.IconPath = value; }
    public Vector2I Coords { get => CoordsBindable.Value; set => CoordsBindable.Value = value; }

    public override bool IsLargeUnit => Creature.Abilities.OfType<AbilityLargeCreature>().Any();
    public int Amount { get => AmountBindable.Value; set => AmountBindable.Value = value; }
    public override double Initiative => CurrentStats.Initiative;
    public double Speed => CurrentStats.Speed;
    public double Defense => CurrentStats.Defense;
    public double MaxHP => Creature.Stats.HitPoints;
    public double TotalHP => Amount > 0 ? CurrentStats.HitPoints + (Amount - 1) * Creature.Stats.HitPoints : 0;
    public int Tier => Creature.Tier;
    public double AverageDamage => (CurrentStats.MinDamage + CurrentStats.MaxDamage) / 2;

    public bool IsOnCoords(Vector2I coords)
    {
        if (!IsLargeUnit) return coords == Coords;

        var delta = coords - Coords;
        return Math.Min(delta.X, delta.Y) >= 0 && Math.Max(delta.X, delta.Y) <= 1;
    }

    public override void Defend()
    {
        Effects.Add(new EffectDefense(this));
    }

    public bool CanCounterattack()
    {
        return AttackedOnThisTurn == 0 || Creature.HasAbility<AbilityUnlimitedRetaliation>();
    }

    public bool CanAttackRanged()
    {
        if (!Creature.IsShooter || CurrentStats.Shots == 0)
            return false;

        var units = BattleHandler.Instance.GetEnemyPlayer(Player)!.AliveArmy;

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

    public AttackType GetAttackType(IAttackable attackable)
    {
        // Don't attack allies
        if (this.IsAlly(attackable))
            return AttackType.None;

        double distanceToTarget = attackable.DistanceTo(this);

        // If it's an enemy - it should be at least 1 tile away
        if (distanceToTarget < 2)
            return Creature.IsShooter ? AttackType.MeleeWithPenalty : AttackType.Melee;

        if (!CanAttackRanged() || CurrentStats.Shots == 0)
            return AttackType.None;

        AttackType result = distanceToTarget <= 6 ? AttackType.RangedStrong : AttackType.RangedWeak;

        return result;
    }

    public AttackParameters CalculateParameters(IAttackable target, bool triggerEvents, bool isCounterattack, MoveResult? moveBeforeAttack = null)
    {
        var parameters = new AttackParameters
        {
            Amount = Amount,
            IsCounterAttack = isCounterattack,
            TriggerEvents = triggerEvents,
            BaseDamage = GD.RandRange(CurrentStats.MinDamage, CurrentStats.MaxDamage),
            WillCounterAttack = !isCounterattack && target.WillCounterattack(this),

            Attack = CurrentStats.Attack,
            Defense = target.Defense,
            AttackType = GetAttackType(target),
            MoveBeforeAttack = moveBeforeAttack
        };

        parameters.IsRanged = parameters.AttackType.IsRanged();

        // Effects before attack
        foreach (var ability in ModifiersOfType<IApplicableBeforeAttack>())
            parameters = ability.Apply(this, target, parameters);

        return parameters;
    }

    public double CalculateDamageFromParameters(AttackParameters parameters)
    {
        // Calculating attack with parameters
        double armorMultiplier = parameters.Attack >= parameters.Defense ?
            (1 + 0.05 * (parameters.Attack - parameters.Defense)) :
            1.0 / (1 + 0.05 * (parameters.Defense - parameters.Attack));

        double damage = parameters.BaseDamage * armorMultiplier * parameters.AttackType.GetMultiplier() * parameters.DamageMultiplier * parameters.Amount;
        return damage;
    }

    public void AttackFromParameters(IAttackable target, AttackParameters parameters)
    {
        double damage = CalculateDamageFromParameters(parameters);

        if (parameters.AttackType.IsRangedShot())
            CurrentStats.Shots--;

        // Attack
        var attackResult = target.TakeDamage(damage, parameters.AttackType, parameters.TriggerEvents);

        // Counterattack
        if (parameters.WillCounterAttack && target is ICanAttack counterAttacker)
            counterAttacker.Attack(this, isCounterattack: true, triggerEvents: parameters.TriggerEvents);

        // Effects after attack
        foreach (var ability in ModifiersOfType<IApplicableAfterAttack>())
            ability.Apply(this, target, parameters, attackResult);
    }

    public bool Attack(IAttackable target, bool triggerEvents, bool isCounterattack, MoveResult? moveBeforeAttack = null)
    {
        if (Amount <= 0) return false;

        AttackParameters parameters = CalculateParameters(target, triggerEvents, isCounterattack, moveBeforeAttack);

        if (parameters.AttackType == AttackType.None)
            return false;

        AttackFromParameters(target, parameters);

        return true;
    }

    public AttackResult CalculateAttackResult(double damage, AttackType attackType)
    {
        // Effects on damage
        foreach (var ability in ModifiersOfType<IApplicableToRecievedDamage>())
            damage = ability.Apply(damage, attackType);

        var result = new AttackResult();

        double incomingDamage = damage;

        // Try to damage HP first
        double absorbedWithHP = Math.Min(damage, CurrentStats.HitPoints);

        damage -= absorbedWithHP;

        // If there's still damage to do - it will kill creatures
        if (damage > 0)
        {
            result.Killed = (int)(damage / Creature.Stats.HitPoints);
            result.Killed = Math.Min(result.Killed, Amount);

            damage -= result.Killed * Creature.Stats.HitPoints;
        }

        if (absorbedWithHP == CurrentStats.HitPoints && result.Killed < Amount)
        {
            result.Killed++;
            damage = 0;
        }

        result.DamageDealt = incomingDamage - damage;

        return result;
    }

    public void ApplyDamage(AttackResult result, bool triggerEvents)
    {
        CurrentStats.HitPoints += result.Killed * Creature.Stats.HitPoints;
        CurrentStats.HitPoints -= result.DamageDealt;

        if (triggerEvents) Amount -= result.Killed;
        else AmountBindable.SetSilent(Amount - result.Killed);


        Debug.Assert(Amount >= 0);

        if (triggerEvents && Amount == 0)
        {
            CreatureDead.Invoke(this);
        }

        AttackedOnThisTurn++;
    }

    public override UnitState SaveState()
    {
        var savedState = new UnitState
        {
            Amount = Amount,
            AttackedOnThisTurn = AttackedOnThisTurn,
            CreatureStats = CurrentStats,
            ATB = ATB,
            Coords = Coords
        };

        // TODO: copy effects

        return savedState;
    }

    public override void LoadState(UnitState savedState, bool silent = true)
    {
        AttackedOnThisTurn = savedState.AttackedOnThisTurn;
        ATB = savedState.ATB;
        CurrentStats = savedState.CreatureStats!.Value;

        if (silent)
        {
            CoordsBindable.SetSilent(savedState.Coords);
            AmountBindable.SetSilent(savedState.Amount);
        }
        else
        {
            Coords = savedState.Coords;
            Amount = savedState.Amount;
        }

        // TODO: load effects
    }
}

public struct UnitState
{
    public int Amount;
    public int AttackedOnThisTurn;
    public CreatureStats? CreatureStats;
    public double ATB;
    public Vector2I Coords;
    public List<Effect>? Effects; // This is nullable because you REALLY woudln't want copying effects unless absolutely necessary
}