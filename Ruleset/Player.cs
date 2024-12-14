using Godot;
using System;

public class Player
{
    public int Id;
    public Color Color;
    public Hero Hero = null!;
    public CreatureInstance?[] Army = new CreatureInstance?[8];
    // War machine units

    public Player()
    {
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

    public static Player Preset1()
    {
        Player player = new Player();

        player.Army[0] = new CreatureInstance(player, new H1g_Militiaman(), 1);
        player.Army[0]!.Coords = new Vector2I(2, 11);

        player.Army[1] = new CreatureInstance(player, new H1_Peasant(), 1);
        player.Army[1]!.Coords = new Vector2I(6, 10);

        return player;
    }

    public static Player Preset2()
    {
        Player player = new Player();

        player.Army[0] = new CreatureInstance(player, new H1g_Militiaman(), 120);
        player.Army[0]!.Coords = new Vector2I(0, 0);

        player.Army[1] = new CreatureInstance(player, new H1g_Militiaman(), 228);
        player.Army[1]!.Coords = new Vector2I(3, 1);

        player.Army[2] = new CreatureInstance(player, new H1g_Militiaman(), 335);
        player.Army[2]!.Coords = new Vector2I(5, 0);

        return player;
    }
}
