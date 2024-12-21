using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Player
{
    public const int MAX_ARMY_SIZE = 8;

    public int Id = 0;
    public Color Color = Colors.Wheat;
    public Hero Hero = null!;
    public readonly CreatureInstance?[] Army = new CreatureInstance?[MAX_ARMY_SIZE];
    private readonly CreatureInstance?[] initialArmy = new CreatureInstance?[MAX_ARMY_SIZE];

    public IEnumerable<CreatureInstance> AliveArmy => Army.Where(c => c != null && c.Amount > 0).Cast<CreatureInstance>();

    public virtual bool UIDrawControls => true;

    public Player()
    {
    }

    /// <summary>
    /// Fires when creature in this player's army is dead.
    /// It's automatically removed from army of this player after event is handled.
    /// </summary>
    public event Action<CreatureInstance> CreatureDied = delegate { };

    public void TriggerUnitDead(CreatureInstance creature) => CreatureDied.Invoke(creature);

    public double CalculateInitialArmyPower() => initialArmy.Select(c => c?.CalculatePower() ?? 0).Sum();

    public CreatureInstance GetInitialStackFor(CreatureInstance creature)
    {
        int index = Army.FirstIndex(c => c == creature);
        return initialArmy[index]!;
    }

    public CreatureInstance? GetCreatureAt(Vector2I coords) => AliveArmy.Where(c => c.IsOnCoords(coords)).FirstOrDefault();

    public CreatureInstance AddCreatureToPlayer(Creature creature, int amount, Vector2I coords, int slot = -1)
    {
        if (slot == -1)
            slot = Army.FirstIndex(c => c is null);

        Debug.Assert(slot != -1 && Army[slot] == null);

        var instance = new CreatureInstance(BattleHandler.Instance, this, creature, amount)
        {
            Coords = coords
        };

        Army[slot] = instance;

        // Make a copy without attachment to BattleHandler
        var instanceCopy = new CreatureInstance(null, this, creature, amount)
        {
            Coords = coords
        };

        initialArmy[slot] = instanceCopy;

        return instance;
    }

    public List<Vector2I> GetPossiblePrePlaningPositions()
    {
        List<Vector2I> result = [];

        if (Id != 1 && Id != 2)
            return result;

        for (int i = 0; i < Playfield.SIZE_X; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                result.Add(new Vector2I(i, Id == 1 ? Playfield.SIZE_Y - j - 1 : j));
            }
        }

        return result;
    }
}

public static class PlayerExtensions
{
    public static bool IsTileOccupiedByPlayer(this Player player, Vector2I tile)
    {
        if (!Playfield.IsInPlayfield(tile, false)) return true;

        foreach (var unit in player.AliveArmy)
        {
            if (unit.IsOnCoords(tile))
                return true;
        }

        return false;
    }

    public static bool IsTileOccupiedByPlayer(this Player player, Vector2I tile, IPlayfieldUnit except)
    {
        if (!Playfield.IsInPlayfield(tile, false)) return true;

        foreach (var unit in player.AliveArmy)
        {
            if (unit == except)
                continue;

            if (unit.IsOnCoords(tile))
                return true;
        }

        return false;
    }
}