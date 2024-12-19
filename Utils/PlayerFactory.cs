using Godot;

public static class PlayerFactory
{
    public static Player CreatePlayer(BattleHandler battleHandler, bool isAI) => isAI ? new AIPlayer(battleHandler) : new Player(battleHandler);
    
    public static Player Preset1(BattleHandler battleHandler, bool isAI = false)
    {
        var player = CreatePlayer(battleHandler, isAI);

        player.Hero = new Hero(player)
        {
            Level = 25,
            IconPath = "res://Assets/Heroes/Haven/Heaven_Freyda_128x128.(Texture).dds",
            BaseStats = new HeroStats
            {
                Attack = 12,
                Defense = 11,
                Spellpower = 6,
                Knowledge = 15
            }
        };

        player.AddCreatureToPlayer(new H2_Archer(), 727, new Vector2I(0, 11));
        player.AddCreatureToPlayer(new H5_Priest(), 69, new Vector2I(1, 11));
        player.AddCreatureToPlayer(new H3_Footman(), 666, new Vector2I(0, 10));
        player.AddCreatureToPlayer(new H1_Peasant(), 1337, new Vector2I(1, 10));

        return player;
    }

    public static Player Preset2(BattleHandler battleHandler, bool isAI = false)
    {
        var player = CreatePlayer(battleHandler, isAI);

        player.Hero = new Hero(player)
        {
            Level = 25,
            IconPath = "res://Assets/Heroes/Haven/Heaven_Godric_128x128.(Texture).dds",
            BaseStats = new HeroStats
            {
                Attack = 9,
                Defense = 13,
                Spellpower = 5,
                Knowledge = 9
            }
        };

        player.AddCreatureToPlayer(new H2a_Crossbowman(), 120, new Vector2I(0, 0));
        player.AddCreatureToPlayer(new H2g_Marksman(), 228, new Vector2I(3, 0));
        player.AddCreatureToPlayer(new H3g_Swordsman(), 100, new Vector2I(1, 1));
        player.AddCreatureToPlayer(new H3a_Vindicator(), 100, new Vector2I(2, 1));
        player.AddCreatureToPlayer(new H5g_Cleric(), 30, new Vector2I(1, 0));
        player.AddCreatureToPlayer(new H5a_Zealot(), 30, new Vector2I(2, 0));

        return player;
    }
}