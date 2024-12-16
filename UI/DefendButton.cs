using Godot;
using System;

public partial class DefendButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Pressed += BattleHandler.Instance.DefendAction;
    }
}
