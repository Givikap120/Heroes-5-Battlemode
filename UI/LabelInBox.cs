
using Godot;

public partial class LabelInBox : Node2D
{
    private Bindable<string> label = new("");
    public string Label 
    { 
        get => label.Value;
        set => label.Value = value; 
    }

    public Font Font { get; set; } = null!;
    public int FontSize { get; set; } = 32;
    public Color BackgroundColor { get; set; } = Colors.Blue;
    public Color BorderColor { get; set; } = Colors.Black;
    public Vector2 Padding { get; set; } = new Vector2(10, 5);

    public override void _Ready()
    {
        label.BindValueChanged(_ => QueueRedraw());
    }

    public override void _Draw()
    {
        if (Font == null || string.IsNullOrEmpty(Label)) return;

        // Calculate text and rectangle sizes
        var textSize = Font.GetStringSize(Label, HorizontalAlignment.Center, fontSize: FontSize);
        var rectSize = textSize + Padding * 2;

        // Draw background rectangle
        DrawRect(new Rect2(Vector2.Zero, rectSize), BackgroundColor);

        // Draw border
        DrawRect(new Rect2(Vector2.Zero, rectSize), BorderColor, filled: false, width: 5);

        // Calculate text position and draw text
        var textPosition = (rectSize - textSize) / 2;
        textPosition.Y += textSize.Y * 3 / 4; // Adjust for baseline alignment
        textPosition.X -= Padding.X / 2;     // Center text better
        DrawString(Font, textPosition, Label, HorizontalAlignment.Center, fontSize: FontSize);
    }
}
