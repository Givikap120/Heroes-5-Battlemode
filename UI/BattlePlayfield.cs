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

    protected override void HandleHoverOnUnit(Vector2I tile, Vector2 mousePos, IPlayfieldUnit unit)
    {
        // If there's tile selected - we need to deselect it first
        if (CurrentlySelectedTile != null)
        {
            SetCell((Vector2I)CurrentlySelectedTile, CurrentlySelectedTileType, Vector2I.Zero);
            CurrentlySelectedTile = null;
        }

        if (CurrentUnit.Value!.IsAlly(unit))
            return;

        CurrentlySelectedTileType = GetCellSourceId(tile);

        if (CurrentlySelectedTileType == (int)TileType.Inactive)
        {
            // Find the relative position of the closest cell except this
            Vector2I hoverDelta = (Vector2I)(mousePos - MapToLocal(tile)).Normalized().Round();
            tile += hoverDelta;
        }

        // Highlight tile with delta
        HighlightTile(tile);
    }

    protected override void HighlightTile(Vector2I tile)
    {
        CurrentlySelectedTileType = GetCellSourceId(tile);

        // If tile doesn't exist - stop
        if (CurrentlySelectedTileType == -1 || CurrentUnit.Value == null)
        {
            CurrentlySelectedTile = null;
            return;
        }

        // Set Tile
        int newTileType = CurrentUnit.Value.DecideTileChange(CurrentlySelectedTileType);

        // If tile is gonna be changed - do this
        if (newTileType > 0)
        {
            CurrentlySelectedTile = tile;
            SetCell(tile, newTileType, Vector2I.Zero);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.Pressed)
            return;

        Vector2I? selectedOrCurrent = CurrentlySelectedTile;

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

            var newDrawableCreature = (DrawableCreatureInstance)item.CreateDrawableRepresentation();
            newDrawableCreature.Scale = new Vector2(2, 2); // Asset of the tile have 2 times higher resolution than icons
            newDrawableCreature.Centered = true;
            newDrawableCreature.Position = MapToLocal(item.Coords);

            item.CoordsBindable.ValueChanged += _ => CallDeferred(nameof(UpdateCreaturePosition), newDrawableCreature);

            Creatures.Add(newDrawableCreature);
            AddChild(newDrawableCreature);
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
        SetCell(movable.Coords, (int)TileType.Select, Vector2I.Zero);

        foreach (var emptyTile in movable.GetPossibleMoveOptions())
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
}
