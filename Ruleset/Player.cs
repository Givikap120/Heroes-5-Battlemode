using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Player
{
    public int Id = 0;
    public Color Color = Colors.Wheat;
    public Hero Hero = null!;
    public readonly CreatureInstance?[] Army = new CreatureInstance?[8];
    private readonly CreatureInstance?[] initialArmy = new CreatureInstance?[8];
    // War machine units

    public IEnumerable<CreatureInstance> AliveArmy => Army.Where(c => c != null && c.Amount > 0).Cast<CreatureInstance>();

    private readonly BattleHandler battleHandler;

    public Player(BattleHandler battleHandler)
    {
        this.battleHandler = battleHandler;
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

    private CreatureInstance addCreatureToPlayer(Creature creature, int amount, Vector2I coords, int slot = -1)
    {
        if (slot == -1)
            slot = Army.FirstIndex(c => c is null);

        Debug.Assert(slot != -1 && Army[slot] == null);

        var instance = new CreatureInstance(battleHandler, this, creature, amount)
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

    public static Player Preset1(BattleHandler battleHandler)
    {
        var player = new Player(battleHandler);

        player.addCreatureToPlayer(new H3_Footman(), 666, new Vector2I(0, 10));
        player.addCreatureToPlayer(new H2_Archer(), 727, new Vector2I(0, 11));
        player.addCreatureToPlayer(new H1_Peasant(), 1337, new Vector2I(1, 10));

        return player;
    }

    public static Player Preset2(BattleHandler battleHandler)
    {
        var player = new Player(battleHandler);

        player.addCreatureToPlayer(new H2a_Crossbowman(), 120, new Vector2I(0, 0));
        player.addCreatureToPlayer(new H2g_Marksman(), 228, new Vector2I(3, 0));
        player.addCreatureToPlayer(new H3g_Swordsman(), 100, new Vector2I(1, 1));
        player.addCreatureToPlayer(new H3a_Vindicator(), 100, new Vector2I(2, 1));

        return player;
    }
}
