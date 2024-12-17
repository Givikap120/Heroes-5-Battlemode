using Godot;
using System.Diagnostics;

public partial class DrawableCreatureInstance : Control
{
    private Sprite2D backgroundSprite = null!;
    private Sprite2D iconSprite = null!;
    private LabelInBox amountLabel = null!;

    private bool? centered = null;
    private double? backgroundSize = null;
    private CreatureInstance parent = null!;
    public Vector2I Coords => parent.Coords;
    private Vector2I sizeI => new(creatureIconTexture?.GetWidth() ?? 0, creatureIconTexture?.GetHeight() ?? 0);
    private Vector2 sizeTrunc => new(creatureIconTexture?.GetWidth() ?? 0, creatureIconTexture?.GetHeight() ?? 0);

    public bool Centered
    {
        get => centered ?? false;
        set
        {
            if (centered == value) return;
            centered = value;
            updatePositions();
        }
    }

    public double BackgroundSize
    {
        get => backgroundSize ?? 1.0;
        set
        {
            if (backgroundSize == value) return;
            backgroundSize = value;
            updatePositions();
        }
    }

    public CreatureInstance Parent 
    {
        get => parent;
        set
        {
            if (parent == value) return;
            unbindFromParent();

            parent = value;

            // Need to load this synchronously so we have the size at init time
            loadIcon(parent.Creature.IconPath);

            updateFromParent();
        }
    }

    private Texture2D creatureIconTexture = null!;

    private void loadIcon(string path)
    {
        
        creatureIconTexture = ResourceLoader.Load<Texture2D>(path);
        Size = new Vector2(creatureIconTexture.GetWidth(), creatureIconTexture.GetHeight());
    }

    

    /// <summary>
    /// For some reason IsNodeReady() can be true even if _Ready() is not called yet.
    /// This thing is fixing this.
    /// </summary>
    private bool isLoadingComplete;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        backgroundSprite = GetNode<Sprite2D>("BackgroundSprite");
        iconSprite = GetNode<Sprite2D>("IconSprite");
        amountLabel = GetNode<LabelInBox>("AmountLabel");

        // Set defaults only if not set before
        centered ??= GetMeta("Centered").AsBool();
        backgroundSize ??= GetMeta("BackgroundSize").AsDouble();

        Debug.Assert(backgroundSize >= 0 && backgroundSize <= 1);

        isLoadingComplete = true;

        if (parent != null) updateFromParent();
    }

    private void updateCreatureAmountAsync(ValueChangedEvent<int> amount) => CallDeferred(nameof(updateCreatureAmount), amount.NewValue);

    private void updateCreatureAmount(int newAmount) => amountLabel.Text = newAmount.ToString();

    private void unbindFromParent()
    {
        if (parent == null) return;
        parent.AmountBindable.ValueChanged -= updateCreatureAmountAsync;
    }

    private void updateFromParent()
    {
        if (!isLoadingComplete) return;

        parent.AmountBindable.ValueChanged += updateCreatureAmountAsync;

        iconSprite.Texture = creatureIconTexture;

        Color baseColor = parent.Player.Color;
        Color tintColor = baseColor * 0.5f;
        tintColor.A = baseColor.A;

        Gradient gradient = new()
        {
            Colors = [baseColor, tintColor]
        };

        GradientTexture2D gradientTexture = new()
        {
            Fill = GradientTexture2D.FillEnum.Radial,
            Gradient = gradient,
            FillFrom = new Vector2(0.5f, 0.5f),
            FillTo = new Vector2(0, 0)
        };

        backgroundSprite.Texture = gradientTexture;

        amountLabel.Text = parent.Amount.ToString();

        updatePositions();
    }

    private void updatePositions()
    {
        if (!isLoadingComplete) return;

        iconSprite.Centered = Centered;
        backgroundSprite.Centered = Centered;

        // Adjust background size
        var backgroundTexture = (GradientTexture2D)backgroundSprite.Texture;
        backgroundTexture.Width = (int)(sizeTrunc.X * BackgroundSize);
        backgroundTexture.Height = (int)(sizeTrunc.Y * BackgroundSize);

        // We also need to offset background if there's no centration
        if (centered == false)
        {
            backgroundSprite.Position = sizeTrunc * (float)(1 - BackgroundSize) / 2;
        }

        amountLabel.Position = Centered ? (sizeTrunc / 2 - amountLabel.Size) : sizeTrunc - amountLabel.Size;
    }
    public override Vector2 _GetMinimumSize() => sizeTrunc;

    public override void _ExitTree()
    {
        unbindFromParent();
        base._ExitTree();
    }
}
