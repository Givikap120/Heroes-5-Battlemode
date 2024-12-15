using Godot;

/// <summary>
/// Text in box with outline.
/// Changing any property except Text will require UpdateLayout function to be called
/// </summary>
public partial class LabelInBox : Control
{
    private ColorRect outlineRect = null!;
    private ColorRect backgroundRect = null!;
    private Label label = null!;

    public readonly Bindable<string> TextBindable = new("");

    public string Text { get => TextBindable.Value; set => TextBindable.Value = value; }
    public Vector2I Padding;
    public int OutlineWidth;
    public int FontSize;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        outlineRect = GetNode<ColorRect>("OutlineRect");
        backgroundRect = GetNode<ColorRect>("BackgroundRect");
        label = GetNode<Label>("Label");

        Text = GetMeta("Text").AsString();
        Padding = GetMeta("Padding").AsVector2I();
        OutlineWidth = GetMeta("OutlineWidth").AsInt32();
        FontSize = GetMeta("FontSize").AsInt32();

        TextBindable.BindValueChanged(_ => UpdateLayout());
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        label.Text = Text;
        label.LabelSettings.FontSize = FontSize;

        Vector2I outlineSize = new Vector2I(OutlineWidth, OutlineWidth);

        var textSize = getTextSize(label);
        var rectSize = textSize + Padding * 2 + outlineSize * 2;

        outlineRect.Size = rectSize;

        backgroundRect.Position = outlineSize;
        backgroundRect.Size = rectSize - outlineSize * 2;

        label.Size = rectSize;

        Size = rectSize;
    }

    private Vector2 getTextSize(Label label)
    {
        var font = label.LabelSettings.Font;
        return font.GetStringSize(label.Text, label.HorizontalAlignment, fontSize: label.LabelSettings.FontSize);
    }
}
