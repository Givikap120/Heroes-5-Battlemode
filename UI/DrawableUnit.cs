using Godot;
using System.Diagnostics;

public partial class DrawableUnit : Control
{
    private Unit parent = null!;

    private Sprite2D backgroundSprite = null!;
    private Sprite2D iconSprite = null!;

    private bool? centered = null;
    private double? backgroundSize = null;
    
    private Vector2I sizeI => new(unitIconTexture?.GetWidth() ?? 0, unitIconTexture?.GetHeight() ?? 0);
    protected Vector2 SizeTrunc => new(unitIconTexture?.GetWidth() ?? 0, unitIconTexture?.GetHeight() ?? 0);

    public bool Centered
    {
        get => centered ?? false;
        set
        {
            if (centered == value) return;
            centered = value;
            UpdatePositions();
        }
    }

    public double BackgroundSize
    {
        get => backgroundSize ?? 1.0;
        set
        {
            if (backgroundSize == value) return;
            backgroundSize = value;
            UpdatePositions();
        }
    }

    public Unit Parent 
    {
        get => parent;
        set
        {
            if (parent == value) return;
            UnbindFromParent();

            parent = value;

            // Need to load this synchronously so we have the size at init time
            loadIcon(parent.IconPath);

            UpdateFromParent();
            UpdatePositions();
        }
    }

    private Texture2D unitIconTexture = null!;

    private void loadIcon(string path)
    {
        unitIconTexture = ResourceLoader.Load<Texture2D>(path);
        Size = new Vector2(unitIconTexture.GetWidth(), unitIconTexture.GetHeight());
    }

    /// <summary>
    /// For some reason IsNodeReady() can be true even if _Ready() is not called yet.
    /// This thing is fixing this.
    /// </summary>
    protected bool IsLoadingComplete;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        backgroundSprite = GetNode<Sprite2D>("BackgroundSprite");
        iconSprite = GetNode<Sprite2D>("IconSprite");

        // Set defaults only if not set before
        centered ??= GetMeta("Centered").AsBool();
        backgroundSize ??= GetMeta("BackgroundSize").AsDouble();

        Debug.Assert(backgroundSize >= 0 && backgroundSize <= 1);

        if (CallLoadingComplete) LoadingComplete();
    }

    protected virtual bool CallLoadingComplete => true;

    protected virtual void LoadingComplete()
    {
        IsLoadingComplete = true;

        if (parent != null)
        {
            UpdateFromParent();
            UpdatePositions();
        }
    }

    protected virtual void UnbindFromParent()
    {
    }

    protected virtual void UpdateFromParent()
    {
        if (!IsLoadingComplete) return;

        iconSprite.Texture = unitIconTexture;

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
    }

    protected virtual void UpdatePositions()
    {
        if (!IsLoadingComplete) return;

        iconSprite.Centered = Centered;
        backgroundSprite.Centered = Centered;

        // Adjust background size
        var backgroundTexture = (GradientTexture2D)backgroundSprite.Texture;
        backgroundTexture.Width = (int)(SizeTrunc.X * BackgroundSize);
        backgroundTexture.Height = (int)(SizeTrunc.Y * BackgroundSize);

        // We also need to offset background if there's no centration
        if (centered == false)
        {
            backgroundSprite.Position = SizeTrunc * (float)(1 - BackgroundSize) / 2;
        }
    }

    public override Vector2 _GetMinimumSize() => SizeTrunc;

    public override void _ExitTree()
    {
        UnbindFromParent();
        base._ExitTree();
    }
}
