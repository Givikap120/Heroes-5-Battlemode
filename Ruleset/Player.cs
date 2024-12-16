﻿using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public class Player
{
    public int Id = 0;
    public Color Color = Colors.Wheat;
    public Hero Hero = null!;
    public CreatureInstance?[] Army = new CreatureInstance?[8];
    // War machine units

    private readonly BattleHandler battleHandler;

    public Player(BattleHandler battleHandler)
    {
        this.battleHandler = battleHandler;

        CreatureDead = creature =>
        {
            int index = Array.FindIndex(Army, c => c == creature);
            if (index >= 0) Army[index] = null;
        };
    }

    /// <summary>
    /// Fires when creature in this player's army is dead.
    /// It's automatically removed from army of this player after event is handled.
    /// </summary>
    public event Action<CreatureInstance> CreatureDead;

    public void TriggerUnitDead(CreatureInstance creature) => CreatureDead.Invoke(creature);

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

        return instance;
    }

    public static Player Preset1(BattleHandler battleHandler)
    {
        var player = new Player(battleHandler);

        player.addCreatureToPlayer(new H1g_Militiaman(), 1, new Vector2I(2, 11));
        player.addCreatureToPlayer(new H1_Peasant(), 1, new Vector2I(6, 10));
        player.addCreatureToPlayer(new H2_Archer(), 727, new Vector2I(0, 11));

        return player;
    }

    public static Player Preset2(BattleHandler battleHandler)
    {
        var player = new Player(battleHandler);

        player.addCreatureToPlayer(new H1g_Militiaman(), 120, new Vector2I(0, 0));
        player.addCreatureToPlayer(new H1g_Militiaman(), 228, new Vector2I(3, 1));
        player.addCreatureToPlayer(new H1g_Militiaman(), 335, new Vector2I(5, 0));

        return player;
    }
}
