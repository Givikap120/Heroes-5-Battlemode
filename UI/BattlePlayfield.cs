using Godot;
using System.Linq;

public partial class BattlePlayfield : Playfield
{
    public Bindable<Unit?> CurrentUnit = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        var battleHandler = BattleHandler.Instance;

        CurrentUnit = battleHandler.CurrentUnit;

        battleHandler.PlayerAdded += _ => SyncPlayers();
        battleHandler.NewTurnStarted += _ => ResetPlayfield();

        SyncPlayers();
    }

    public override void ResetPlayfield()
    {
        base.ResetPlayfield();

        if (CurrentUnit?.Value != null && CurrentUnit.Value.Player.UIDrawControls) addUnitActions(CurrentUnit.Value);
    }

    protected override void HandleHoverOnSpace(Vector2I tile)
    {
        if (CurrentUnit.Value == null)
            return;

        // No need to update anything if tile is the same
        if (tile == CurrentlySelectedTile?.Closest)
            return;

        // If there's tile selected - we need to deselect it first
        if (CurrentlySelectedUnit == null)
            DeselectCurrentTile();

        bool isLarge = CurrentUnit.Value.IsLargeUnit;
        var shifted = ICanMove.ShiftMoveTileIfOccupied(tile, isLarge);

        if (!IsInPlayfield(shifted.Full, isLarge))
            return;

        HighlightTile(shifted);
    }

    protected override void HandleHoverOnUnit(Vector2I tile, Vector2 mousePos, IPlayfieldUnit? previousUnit)
    {
        if (CurrentlySelectedUnit == null)
            return;

        // If there's tile selected - we need to deselect it first
        DeselectCurrentTile();

        if (CurrentUnit.Value!.IsAlly(CurrentlySelectedUnit))
            return;

        var target = CurrentlySelectedUnit.Coords;

        if (GetCellSourceId(tile) == (int)TileType.Inactive)
        {
            bool isLarge = CurrentlySelectedUnit.IsLargeUnit;
            Vector2 creatureCenter = isLarge
                ? (MapToLocal(CurrentlySelectedUnit.Coords) + MapToLocal(CurrentlySelectedUnit.Coords + Vector2I.One)) / 2
                : MapToLocal(CurrentlySelectedUnit.Coords);

            // Find the relative position of the closest cell except this
            Vector2I hoverDelta = (Vector2I)(mousePos - creatureCenter).Normalized().Round();
            tile += hoverDelta;

            if (isLarge)
            {
                if (tile.X < CurrentlySelectedUnit.Coords.X)
                    tile.X--;

                if (tile.Y < CurrentlySelectedUnit.Coords.Y)
                    tile.Y--;
            }

            HandleHoverOnSpace(tile);
            return;
        }

        // Highlight tile with delta
        HighlightTile((target, tile));
    }

    protected override void HighlightTile((Vector2I Full, Vector2I Closest) tile)
    {
        int currentTileType = GetCellSourceId(tile.Closest);

        // If tile doesn't exist - stop
        if (currentTileType == -1 || CurrentUnit.Value == null)
        {
            CurrentlySelectedTile = null;
            return;
        }

        // Set Tile
        int newTileType = CurrentUnit.Value.DecideTileChange(currentTileType);

        // If tile is gonna be changed - do this
        if (newTileType > 0)
        {
            CurrentlySelectedTile = tile;
            SetCellCustom(tile.Full, newTileType);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.Pressed)
            return;

        Vector2I? selectedOrCurrent = CurrentlySelectedTile?.Full;

        if (CurrentUnit.Value is IPlayfieldUnit playfieldUnit)
        {
            selectedOrCurrent ??= playfieldUnit.Coords;
        }

        if (selectedOrCurrent == null)
            return;

        // If no unit selected - just select cell
        if (CurrentlySelectedUnit == null) BattleHandler.Instance.EmptyCellAction(selectedOrCurrent.Value);

        // If unit is the cell selection - cell is not important
        else if (CurrentlySelectedUnit.Coords == selectedOrCurrent) BattleHandler.Instance.UnitAction(CurrentlySelectedUnit);

        // Else it's double action
        else BattleHandler.Instance.UnitWithCellAction(CurrentlySelectedUnit, selectedOrCurrent.Value);
    }

    private readonly Player? player1 = null;
    private readonly Player? player2 = null;
    
    public void SyncPlayers()
    {
        var handlerPlayer1 = BattleHandler.Instance.Player1;
        var handlerPlayer2 = BattleHandler.Instance.Player2;

        if (player1 == null && handlerPlayer1 != null) addPlayer(handlerPlayer1);
        if (player2 == null && handlerPlayer2 != null) addPlayer(handlerPlayer2);
    }

    private void addPlayer(Player player)
    {
        foreach (var item in player.Army)
        {
            if (item == null)
                continue;

            var newDrawableCreature = AddDrawableCreature(item);
            item.CoordsBindable.ValueChanged += _ => CallDeferred(nameof(UpdateCreaturePosition), newDrawableCreature);
        }

        player.CreatureDied += creature => CallDeferred(nameof(handleCreatureDead), creature);
    }

    private void handleCreatureDead(CreatureInstance creature)
    {
        var deadCreatures = Creatures.Where(c => c.Parent == creature);

        foreach (var deadCreature in deadCreatures)
        {
            RemoveChild(deadCreature);
        }

        Creatures.RemoveAll(c => c.Parent == creature);
    }

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
        SetBaseCellCustom(movable.Coords, movable.IsLargeUnit ? (int)TileType.SelectBig : (int)TileType.Select);

        foreach (var emptyTile in movable.GetPossibleMoveOptions())
        {
            SetBaseCellCustom(emptyTile, (int)TileType.Affected);
        }
    }

    private void addShootVariants(ICanAttack shooter)
    {
        foreach (var creature in BattleHandler.Instance.GetEnemyPlayer(shooter.Player)!.AliveArmy)
        {
            if (shooter.GetAttackType(creature).IsRanged())
            {
                SetBaseCellCustom(creature.Coords, creature.IsLargeUnit ? (int)TileType.AimableBig : (int)TileType.Aimable);
            }
        }
    }
}
