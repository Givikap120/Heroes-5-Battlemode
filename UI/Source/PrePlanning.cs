using Godot;

public partial class PrePlanning : Node2D
{
    public PrePlanning()
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(new CursorHandler());
        PrePlanningHandler.Instance.StartPrePlanning();
    }
}
