using Godot;

public partial class DrawableCreatureInstance : Node2D
{
    public CreatureInstance Parent { get; private set; }
    public DrawableCreatureInstance(CreatureInstance parent)
    {
        Parent = parent;
        parent.AmountBindable.BindValueChanged(amount => 
            amountBox.Label = amount.ToString()
            );
    }

    private Font font = null!;
    private Sprite2D backgroundSprite = new() { ZIndex = 1 };
    private Sprite2D iconSprite = new() { ZIndex = 2 };
    private LabelInBox amountBox = new() { ZIndex = 3 };

    public Vector2I Coords => Parent.Coords;

    public override void _Ready()
    {
        // Load the font resource
        font = ResourceLoader.Load<Font>("res://Assets/Monotype-Corsiva-Regular.ttf");

        // Create and configure the Sprite2D for the icon
        iconSprite.Texture = ResourceLoader.Load<Texture2D>(Parent.Creature.IconPath);

        Color baseColor = Parent.Player.Color;
        Color tintColor = baseColor * 0.5f;

        // Add radial gradient effect
        Gradient gradient = new Gradient()
        {
            Colors = [baseColor, tintColor]
        };

        GradientTexture2D gradientTexture = new GradientTexture2D
        {
            Fill = GradientTexture2D.FillEnum.Radial,
            Gradient = gradient,
            Width = (int)(iconSprite.Texture.GetWidth() * 0.8),
            Height = (int)(iconSprite.Texture.GetHeight() * 0.8),
            FillFrom = new Vector2(0.5f, 0.5f),
            FillTo = new Vector2(0, 0)
        };

        backgroundSprite.Texture = gradientTexture;
        //iconSprite.Material = ResourceLoader.Load<ShaderMaterial>("res://Materials/OutlineShaderMaterial.tres");

        // Add the background and sprite to the parent node
        AddChild(backgroundSprite);

        AddChild(iconSprite);

        amountBox.FontSize = 42;
        amountBox.Font = font;
        amountBox.Label = Parent.AmountBindable.Value.ToString();
        AddChild(amountBox);
    }

    public new Vector2 Position { 
        set 
        {
            backgroundSprite.Position = value;
            iconSprite.Position = value;
            amountBox.Position = value;
        }
    }

    public new Vector2 Scale
    {
        set
        {
            backgroundSprite.Scale = value;
            iconSprite.Scale = value;
            amountBox.Scale = value;
        }
    }
}

