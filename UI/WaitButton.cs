using Godot;
using System;

public partial class WaitButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Pressed += BattleHandler.Instance.WaitAction;
    }
}
