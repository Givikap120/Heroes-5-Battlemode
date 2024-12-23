using Godot;

public partial class Logger : Control
{
    private ScrollContainer scrollContainer = null!;
    private VBoxContainer internalContainer = null!;

    private LabelSettings labelSettingsSource = null!;

    private bool shouldScroll = false;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (shouldScroll)
        {
            scrollContainer.ScrollVertical = (int)scrollContainer.GetVScrollBar().MaxValue;
            shouldScroll = false;
        }
    }


    public override void _Ready()
	{
        scrollContainer = GetNode<ScrollContainer>("%ScrollContainer");
        internalContainer = GetNode<VBoxContainer>("%InternalContainer");

        var font = GD.Load<Font>("res://Assets/Monotype-Corsiva-Regular.ttf");
        int fontSize = GetMeta("FontSize").As<int>();

        labelSettingsSource = new LabelSettings
        {
            Font = font,
            FontSize = fontSize,
            OutlineSize = fontSize / 4,
            OutlineColor = Colors.Black
        };

        int margin = GetMeta("Margin").As<int>();
        MarginContainer marginContainer = GetNode<MarginContainer>("Margin");

        marginContainer.AddThemeConstantOverride("margin_top", margin);
        marginContainer.AddThemeConstantOverride("margin_left", margin);
        marginContainer.AddThemeConstantOverride("margin_bottom", margin);
        marginContainer.AddThemeConstantOverride("margin_right", margin);

        bindToBattleHandler();
    }

    private void bindToBattleHandler()
    {
        var battleHandler = BattleHandler.Instance;

        battleHandler.OnMove += logMove;
        battleHandler.OnAttack += logAttack;
        battleHandler.OnDefense += logDefense;
        battleHandler.OnWait += logWait;
    }
    private void logMessage(string message, Color color)
    {
        var labelSettings = labelSettingsSource.Copy();
        labelSettings.FontColor = color;

        var messageLabel = new Label
        {
            Text = message,
            LabelSettings = labelSettings
        };

        internalContainer.AddChild(messageLabel);
        shouldScroll = true;
    }

    public void LogMessage(string message, Color color) => CallDeferred(nameof(logMessage), message, color);

    private void logMove(MoveResult move)
    {
        LogMessage($"{move.Actor.Name}: Move from {move.Before} to {move.After}", move.Actor.Player.Color);
    }

    private void logAttack(AttackResult attack)
    {
        string attackWord = attack.AttackParameters.IsCounterAttack ? "Counterattack" : "Attack";
        LogMessage($"{attack.Attacker.Name}: {attackWord} {attack.Target.Name}, Damage dealt -  {(int)attack.DamageDealt} ({attack.Killed} killed)", attack.Attacker.Player.Color);
    }

    private void logDefense(Unit unit)
    {
        LogMessage($"{unit.Name}: Defense", unit.Player.Color);
    }

    private void logWait(Unit unit)
    {
        LogMessage($"{unit.Name}: Wait", unit.Player.Color);
    }
}

public static class LabelSettingsExtensions
{
    public static LabelSettings Copy(this LabelSettings labelSettings)
    {
        return new LabelSettings
        {
            Font = labelSettings.Font,
            FontSize = labelSettings.FontSize,
            OutlineSize = labelSettings.OutlineSize,
            OutlineColor = labelSettings.OutlineColor
        };
    }
}
