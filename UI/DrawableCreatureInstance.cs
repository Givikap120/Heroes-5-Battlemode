using Godot;

public partial class DrawableCreatureInstance : DrawableUnit
{
    public CreatureInstance ParentCreature => (CreatureInstance)Parent;

    private LabelInBox amountLabel = null!;
    public Vector2I Coords => ParentCreature.Coords;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        amountLabel = GetNode<LabelInBox>("AmountLabel");

        LoadingComplete();
    }

    protected override bool CallLoadingComplete => false;

    private void updateCreatureAmountAsync(ValueChangedEvent<int> amount) => CallDeferred(nameof(updateCreatureAmount), amount.NewValue);

    private void updateCreatureAmount(int newAmount) => amountLabel.Text = newAmount.ToString();

    protected override void UnbindFromParent()
    {
        if (Parent == null) return;

        base.UnbindFromParent();

        ParentCreature.AmountBindable.ValueChanged -= updateCreatureAmountAsync;
    }

    protected override void UpdateFromParent()
    {
        if (!IsLoadingComplete) return;

        base.UpdateFromParent();

        ParentCreature.AmountBindable.ValueChanged += updateCreatureAmountAsync;
        amountLabel.Text = ParentCreature.Amount.ToString();
    }

    protected override void UpdatePositions()
    {
        if (!IsLoadingComplete) return;

        base.UpdatePositions();

        amountLabel.Position = Centered ? (SizeTrunc / 2 - amountLabel.Size) : SizeTrunc - amountLabel.Size;
    }
}
