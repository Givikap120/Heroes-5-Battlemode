using Godot;
using System.Linq;

public partial class PrePlanningPlayfield : Playfield
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        currentPlayer = PrePlanningHandler.Instance.CurrentPlayer;
        currentPlayer.BindValueChanged(p => ResetPlayfield(), true);
    }

    private Bindable<Player?> currentPlayer = null!;

    public override void ResetPlayfield()
    {
        base.ResetPlayfield();

        if (currentPlayer?.Value != null)
        {
            resetChildren(currentPlayer.Value);
            drawPositionVariants(currentPlayer.Value);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (currentlyDraggedCreature != null)
        {
            var mousePos = ToLocal(GetGlobalMousePosition());
            currentlyDraggedCreature.Position = mousePos;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left)
            return;

        if (!mouseEvent.Pressed)
        {
            handleDragEnd();
            return;
        }

        if (CurrentlySelectedTile != null && mouseEvent.Pressed)
        {
            handleDragStart();
            return;
        }
    }

    private DrawableCreatureInstance? currentlyDraggedCreature = null;

    private void handleDragStart()
    {
        // No drag if no unit selected
        if (CurrentlySelectedUnit == null || CurrentlySelectedUnit is not CreatureInstance creature)
            return;

        // If there's already something dragged - drop first
        if (currentlyDraggedCreature != null)
            handleDragEnd();

        currentlyDraggedCreature = Creatures.Where(c => c.ParentCreature == creature).First();
        currentlyDraggedCreature.ZIndex = 1;
        CurrentlySelectedUnit = null;
    }

    private void handleDragEnd()
    {
        if (currentlyDraggedCreature == null)
            return;

        if (CurrentlySelectedTile != null && CurrentlySelectedUnit == null)
        {
            currentlyDraggedCreature.ParentCreature.Coords = (Vector2I)CurrentlySelectedTile;
        }

        UpdateCreaturePosition(currentlyDraggedCreature);
        currentlyDraggedCreature.ZIndex = 0;
        currentlyDraggedCreature = null;
    }

    protected override void HighlightTile(Vector2I tile)
    {
        CurrentlySelectedTileType = GetCellSourceId(tile);

        // If tile doesn't exist - stop
        if (CurrentlySelectedTileType == -1)
        {
            CurrentlySelectedTile = null;
            return;
        }

        // If tile is gonna be changed - do this
        if (CurrentlySelectedTileType == (int)TileType.Affected)
        {
            CurrentlySelectedTile = tile;
            SetCell(tile, (int)TileType.Select, Vector2I.Zero);
        }
    }

    private void resetChildren(Player player)
    {
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        foreach (var item in player.Army)
        {
            if (item == null)
                continue;

            var newDrawableCreature = (DrawableCreatureInstance)item.CreateDrawableRepresentation();
            newDrawableCreature.Scale = new Vector2(2, 2); // Asset of the tile have 2 times higher resolution than icons
            newDrawableCreature.Centered = true;
            newDrawableCreature.Position = MapToLocal(item.Coords);

            Creatures.Add(newDrawableCreature);
            AddChild(newDrawableCreature);
        }
    }

    private void drawPositionVariants(Player player)
    {
        foreach (var tile in player.GetPossiblePrePlaningPositions())
        {
            SetCell(tile, (int)TileType.Affected, Vector2I.Zero);
        }
    }
}
