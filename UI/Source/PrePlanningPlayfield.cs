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

        var creature = (IPlayfieldUnit)currentlyDraggedCreature.Parent;

        if (CurrentlySelectedTile != null && CurrentlySelectedUnit == null && creature.CanBePlacedOnTile(CurrentlySelectedTile.Value.Full))
        {
            creature.Coords = CurrentlySelectedTile.Value.Full;
        }

        UpdateCreaturePosition(currentlyDraggedCreature);
        currentlyDraggedCreature.ZIndex = 0;
        currentlyDraggedCreature = null;
    }

    protected override void HighlightTile((Vector2I Full, Vector2I Closest) tile)
    {
        int currentTileType = GetCellSourceId(tile.Full);

        // If tile doesn't exist - stop
        if (currentTileType == -1)
        {
            CurrentlySelectedTile = null;
            return;
        }

        // If tile is gonna be changed - do this
        if (currentTileType == (int)TileType.Affected)
        {
            CurrentlySelectedTile = tile;

            IPlayfieldUnit? currentCreature = (IPlayfieldUnit?)currentlyDraggedCreature ?? CurrentlySelectedUnit;
            bool isLarge = currentCreature?.IsLargeUnit ?? false;

            SetCellCustom(tile.Full, isLarge ? (int)TileType.SelectBig : (int)TileType.Select);
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

            AddDrawableCreature(item);
        }
    }

    private void drawPositionVariants(Player player)
    {
        foreach (var tile in player.GetPossiblePrePlaningPositions())
        {
            SetBaseCellCustom(tile, (int)TileType.Affected);
        }
    }
}
