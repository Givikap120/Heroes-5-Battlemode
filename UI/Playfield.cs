using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class Playfield : TileMapLayer
{
	public const int SIZE_X = 10;
	public const int SIZE_Y = 12;

    public enum TileType
    {
        Inactive,
        Affected,
        AffectedBig,
        Select,
        SelectBig,
        Aimable,
        AimableBig,
        Intersection
    }

    public Bindable<Unit?> CurrentUnit = null!;

    private Vector2I? currentlySelectedTile;
    private int currentlySelectedTileType = -1;
    private IPlayfieldUnit? currentlySelectedUnit;
    private readonly List<DrawableCreatureInstance> creatures = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ResetPlayfield();

        var battleHandler = BattleHandler.Instance;

        CurrentUnit = battleHandler.CurrentUnit;

        battleHandler.PlayerAdded += AddPlayer;
        battleHandler.NewTurnStarted += _ => ResetPlayfield();
    }

    public void ResetPlayfield()
    {
        for (int i = 0; i < SIZE_X; i++)
        {
            for (int j = 0; j < SIZE_Y; j++)
            {
                SetCell(new Vector2I(i, j), (int)TileType.Inactive, Vector2I.Zero);
            }
        }
        currentlySelectedTile = null;

        if (CurrentUnit?.Value != null && CurrentUnit.Value.Player.UIDrawControls) addUnitActions(CurrentUnit.Value);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        // Don't update UI if there's no unit selected
        if (CurrentUnit?.Value == null)
            return;

        var mousePos = ToLocal(GetGlobalMousePosition());
        var tile = LocalToMap(mousePos);

        // Decide the branch
        currentlySelectedUnit = GetPlayfieldEntityAt(tile);
        if (currentlySelectedUnit == null) handleHoverOnSpace(tile);
        else handleHoverOnUnit(tile, mousePos, currentlySelectedUnit);
    }

    private void handleHoverOnSpace(Vector2I tile)
    {
        // No need to update anything if tile is the same
        if (tile == currentlySelectedTile)
            return;

        // If there's tile selected - we need to deselect it first
        if (currentlySelectedTile != null)
        {
            SetCell((Vector2I)currentlySelectedTile, currentlySelectedTileType, Vector2I.Zero);
            currentlySelectedTile = null;
        }

        highlightTile(tile);
    }

    private void handleHoverOnUnit(Vector2I tile, Vector2 mousePos, IPlayfieldUnit unit)
    {
        // If there's tile selected - we need to deselect it first
        if (currentlySelectedTile != null)
        {
            SetCell((Vector2I)currentlySelectedTile, currentlySelectedTileType, Vector2I.Zero);
            currentlySelectedTile = null;
        }

        if (CurrentUnit.Value!.IsAlly(unit))
            return;

        currentlySelectedTileType = GetCellSourceId(tile);

        if (currentlySelectedTileType == (int)TileType.Inactive)
        {
            // Find the relative position of the closest cell except this
            Vector2I hoverDelta = (Vector2I)(mousePos - MapToLocal(tile)).Normalized().Round();
            tile += hoverDelta;
        }

        // Highlight tile with delta
        highlightTile(tile);
    }

    private void highlightTile(Vector2I tile)
    {
        currentlySelectedTileType = GetCellSourceId(tile);

        // If tile doesn't exist - stop
        if (currentlySelectedTileType == -1)
        {
            currentlySelectedTile = null;
            return;
        }

        // Set Tile
        int newTileType = CurrentUnit.Value!.DecideTileChange(currentlySelectedTileType);

        // If tile is gonna be changed - do this
        if (newTileType > 0)
        {
            currentlySelectedTile = tile;
            SetCell(tile, newTileType, Vector2I.Zero);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.Pressed)
            return;

        Vector2I? selectedOrCurrent = currentlySelectedTile;

        if (CurrentUnit.Value is IPlayfieldUnit playfieldUnit)
        {
            selectedOrCurrent ??= playfieldUnit.Coords;
        }

        if (selectedOrCurrent == null)
        {
            return;
        }
        else
        {
            // If no unit selected - just select cell
            if (currentlySelectedUnit == null) BattleHandler.Instance.EmptyCellAction(selectedOrCurrent.Value);

            // If unit is the cell selection - cell is not important
            else if(currentlySelectedUnit.Coords == selectedOrCurrent) BattleHandler.Instance.UnitAction(currentlySelectedUnit);

            // Else it's double action
            else BattleHandler.Instance.UnitWithCellAction(currentlySelectedUnit, selectedOrCurrent.Value);
        }
    }

    public void AddPlayer(Player player)
    {
        foreach (var item in player.Army)
        {
            if (item == null)
                continue;

            var newDrawableCreature = item.CreateDrawableRepresentation();
            newDrawableCreature.Scale = new Vector2(1.0f / Scale.X, 1.0f / Scale.Y);
            newDrawableCreature.Centered = true;
            newDrawableCreature.Position = MapToLocal(item.Coords);

            item.CoordsBindable.ValueChanged += _ => CallDeferred(nameof(updateCreaturePosition), newDrawableCreature);

            creatures.Add(newDrawableCreature);
            AddChild(newDrawableCreature);
        }

        player.CreatureDied += creature => CallDeferred(nameof(handleCreatureDead), creature);
    }

    private void updateCreaturePosition(DrawableCreatureInstance drawable)
    {
        drawable.Position = MapToLocal(drawable.Parent.Coords);
    }

    private void handleCreatureDead(CreatureInstance creature)
    {
        var deadCreatures = creatures.Where(c => c.Parent == creature);

        foreach (var deadCreature in deadCreatures)
        {
            RemoveChild(deadCreature);
        }

        creatures.RemoveAll(c => c.Parent == creature);
    }

    public CreatureInstance? GetPlayfieldEntityAt(Vector2I tile) => creatures.FirstOrDefault(creature => creature.Coords == tile)?.Parent;

    private void addUnitActions(Unit? unit)
    {
        if (unit == null)
            return;

        if (unit is ICanMove movable)
            addMoveVariants(movable);

        if (unit is ICanAttack attacker && attacker.CanAttackRanged())
            addShootVariants(attacker);
    }

    private void addMoveVariants(ICanMove movable)
    {
        SetCell(movable.Coords, (int)TileType.Select, Vector2I.Zero);

        foreach (var emptyTile in movable.GetPossibleMoveOptions(c => GetPlayfieldEntityAt(c) != null))
        {
            SetCell(emptyTile, (int)TileType.Affected, Vector2I.Zero);
        }
    }

    private void addShootVariants(ICanAttack shooter)
    {
        foreach (var creature in BattleHandler.Instance.GetEnemyPlayer(shooter.Player)!.AliveArmy)
        {
            if (shooter.GetAttackType(creature).IsRanged())
            {
                SetCell(creature.Coords, (int)TileType.Aimable, Vector2I.Zero);
            }   
        }
    }

    public static bool IsInPlayfield(Vector2I coords) => coords.X >= 0 && coords.Y >= 0 && coords.X < SIZE_X && coords.Y < SIZE_Y;
}
