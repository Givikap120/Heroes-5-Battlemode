using Godot;

public partial class Battle : Node2D
{
    private Logger logger = null!;

    public Battle()
    {
        BattleHandler.Instance.GameEnded += callGameOverWindow;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        logger = GetNode<Logger>("%Logger");

        AddChild(new CursorHandler());
        BattleHandler.Instance.StartGame();

        logger.LogMessage("Battle Started.", Colors.White);
    }

    private void callGameOverWindow(string text)
    {
        GD.Print($"Game Over. {text}");
        logger.LogMessage($"Game Over. {text}", Colors.White);

        var popup = GetNode<PopupPanel>("GameOverPopup");

        var label = popup.GetNode<Label>("GameOverPopupText");
        label.Text = text;

        popup.Show();
    }
}
