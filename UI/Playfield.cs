using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public abstract partial class Playfield : TileMapLayer
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

    protected Vector2I? CurrentlySelectedTile;
    protected int CurrentlySelectedTileType = -1;
    protected IPlayfieldUnit? CurrentlySelectedUnit;
    protected readonly List<DrawableCreatureInstance> Creatures = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ResetPlayfield();

        adjustPlayfieldPositionAsync();
    }

    private void adjustPlayfieldPositionAsync()
    {
        var control = GetParent<Control>();

        var globalScale = ToGlobal(Vector2.One) - ToGlobal(Vector2.Zero);

        var mapSize = new Vector2(SIZE_X, SIZE_Y);
        var localSize = mapSize * TileSet.TileSize;
        var globalSize = localSize * globalScale;

        // First center it
        control.Size = globalSize;
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterLeft, Control.LayoutPresetMode.KeepSize);

        // After centering it first time - find Y offset and set it as margin
        int margin = (int)control.GetScreenPosition().Y;
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterLeft, Control.LayoutPresetMode.KeepSize, margin);
    }

    public virtual void ResetPlayfield()
    {
        for (int i = 0; i < SIZE_X; i++)
        {
            for (int j = 0; j < SIZE_Y; j++)
            {
                SetCell(new Vector2I(i, j), (int)TileType.Inactive, Vector2I.Zero);
            }
        }
        CurrentlySelectedTile = null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        var mousePos = ToLocal(GetGlobalMousePosition());
        var tile = LocalToMap(mousePos);

        // Decide the branch
        CurrentlySelectedUnit = GetPlayfieldEntityAt(tile);
        if (CurrentlySelectedUnit == null) handleHoverOnSpace(tile);
        else HandleHoverOnUnit(tile, mousePos, CurrentlySelectedUnit);
    }

    private void handleHoverOnSpace(Vector2I tile)
    {
        // No need to update anything if tile is the same
        if (tile == CurrentlySelectedTile)
            return;

        // If there's tile selected - we need to deselect it first
        if (CurrentlySelectedTile != null)
        {
            SetCell((Vector2I)CurrentlySelectedTile, CurrentlySelectedTileType, Vector2I.Zero);
            CurrentlySelectedTile = null;
        }

        HighlightTile(tile);
    }

    protected virtual void HandleHoverOnUnit(Vector2I tile, Vector2 mousePos, IPlayfieldUnit unit)
    {
        // If there's tile selected - we need to deselect it first
        if (CurrentlySelectedTile != null)
        {
            SetCell((Vector2I)CurrentlySelectedTile, CurrentlySelectedTileType, Vector2I.Zero);
            CurrentlySelectedTile = null;
        }

        HighlightTile(tile);
    }

    protected void UpdateCreaturePosition(DrawableCreatureInstance drawable)
    {
        drawable.Position = MapToLocal(drawable.ParentCreature.Coords);
    }

    protected abstract void HighlightTile(Vector2I tile);

    public CreatureInstance? GetPlayfieldEntityAt(Vector2I tile) => Creatures.FirstOrDefault(creature => creature.Coords == tile)?.ParentCreature;

    public static bool IsInPlayfield(Vector2I coords) => coords.X >= 0 && coords.Y >= 0 && coords.X < SIZE_X && coords.Y < SIZE_Y;
}
