using Godot;
using System;

public partial class Main : Node2D
{
    private readonly BattleHandler battleHandler;

    public Main()
    {
        battleHandler = new BattleHandler(this);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        battleHandler.StartGame();
    }
}
