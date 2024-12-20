using Godot;

public partial class Battle : Node2D
{
    public Battle()
    {
        BattleHandler.Instance.GameEnded += callGameOverWindow;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BattleHandler.Instance.StartGame();
    }

    private void callGameOverWindow(string text)
    {
        GD.Print($"Game Over. {text}");

        var popup = GetNode<PopupPanel>("GameOverPopup");

        var label = popup.GetNode<Label>("GameOverPopupText");
        label.Text = text;

        popup.Show();
    }
}
