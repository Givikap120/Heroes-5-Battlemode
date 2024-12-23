using Godot;

public partial class DamageTooltip : Control
{
    private Label tooltipText = null!;
    private bool shouldTrackMouse = false;

    public override void _Ready()
    {
        base._Ready();

        tooltipText = GetNode<Label>("TooltipText");
    }

    public override void _Process(double delta)
    {
        if (!shouldTrackMouse)
            return;

        Position = GetParent<Node2D>().ToLocal(GetGlobalMousePosition() + new Vector2(8f, 32f));
    }

    private ICanAttack? previousAttacker;
    private IAttackable? previousTarget;
    private MoveResult? previousMoveResult;

    public void Show(ICanAttack attacker, IAttackable target, MoveResult? moveResult)
    {
        // Don't recalculate if nothing changed
        if (attacker == previousAttacker && target == previousTarget && moveResult == previousMoveResult)
        {
            shouldTrackMouse = true;
            Show();
            return;
        }

        previousAttacker = attacker;
        previousTarget = target;
        previousMoveResult = moveResult;

        var baseParameters = attacker.CalculateParameters(target, triggerEvents: false, moveBeforeAttack: moveResult);

        var minParameters = baseParameters;
        minParameters.BaseDamage = double.IsNaN(attacker.MinDamage) ? minParameters.BaseDamage : attacker.MinDamage;

        var maxParameters = baseParameters;
        maxParameters.BaseDamage = double.IsNaN(attacker.MaxDamage) ? minParameters.BaseDamage : attacker.MaxDamage;

        var minResult = target.CalculateAttackResult(attacker.CalculateDamageFromParameters(minParameters), minParameters.AttackType);
        var maxResult = target.CalculateAttackResult(attacker.CalculateDamageFromParameters(maxParameters), maxParameters.AttackType);

        static string getValueRangeDisplay(int a, int b) => a == b ? $"{a}" : $"{a} - {b}";

        string damageText = $"Potential damage: {getValueRangeDisplay((int)minResult.DamageDealt, (int)maxResult.DamageDealt)}\n";
        string killsText = $"Potential kills: {getValueRangeDisplay(minResult.Killed, maxResult.Killed)}\n";
        string counterattackText = $"Counterattack: {(baseParameters.WillCounterAttack ? "Yes" : "No")}\n";

        tooltipText.Text = damageText + killsText + counterattackText;
    }

    public void HideTooltip()
    {
        shouldTrackMouse = false;
        Hide();
    }
}
