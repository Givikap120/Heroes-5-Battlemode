using Godot;
using System;

public partial class Main : Node2D
{
    public readonly BattleHandler BattleHandler;

    public Main()
    {
        BattleHandler = new BattleHandler(this);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BattleHandler.StartGame();
    }
}
