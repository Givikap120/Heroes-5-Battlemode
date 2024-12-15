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
            parent = value;
            updateFromParent();
        }
    }

    public Vector2I SizeI => new((int)Size.X, (int)Size.Y);

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

    private void updateFromParent()
    {
        if (!isLoadingComplete) return;

        parent.AmountBindable.BindValueChanged(amount => amountLabel.Text = amount.ToString());

        iconSprite.Texture = ResourceLoader.Load<Texture2D>(parent.Creature.IconPath);
        Size = new Vector2(iconSprite.Texture.GetWidth(), iconSprite.Texture.GetHeight());

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

        amountLabel.Text = parent.AmountBindable.Value.ToString();

        updatePositions();
    }

    private void updatePositions()
    {
        if (!isLoadingComplete) return;

        iconSprite.Centered = Centered;
        backgroundSprite.Centered = Centered;

        // Adjust background size
        var backgroundTexture = (GradientTexture2D)backgroundSprite.Texture;
        backgroundTexture.Width = (int)(Size.X * BackgroundSize);
        backgroundTexture.Height = (int)(Size.Y * BackgroundSize);

        // We also need to offset background if there's no centration
        if (centered == false)
        {
            Vector2 sizeTrunc = SizeI;
            backgroundSprite.Position = sizeTrunc * (float)(1 - BackgroundSize) / 2;
        }

        amountLabel.Position = Centered ? Vector2.Zero : Size - amountLabel.Size;
    }
}
