using Godot;
using System.Collections.Generic;
using System.Linq;
using static Playfield;

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

    /// <summary>
    /// Full is the actual selected tile to work with.
    /// Closest is the file that was hovered on before real selected tile was calculated.
    /// </summary>
    protected (Vector2I Full, Vector2I Closest)? CurrentlySelectedTile;

    protected IPlayfieldUnit? CurrentlySelectedUnit;
    protected readonly List<DrawableCreatureInstance> Creatures = [];

    protected int[,] BaseSourceIds = new int[SIZE_X, SIZE_Y];

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
                SetBaseCellCustom(new Vector2I(i, j), (int)TileType.Inactive);
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
        var previousUnit = CurrentlySelectedUnit;
        CurrentlySelectedUnit = GetPlayfieldEntityAt(tile);
        if (CurrentlySelectedUnit == null) HandleHoverOnSpace(tile);
        else HandleHoverOnUnit(tile, mousePos, previousUnit);
    }

    protected void DeselectCurrentTile()
    {
        if (CurrentlySelectedTile != null)
        {
            ResetCell(CurrentlySelectedTile.Value.Full);
            CurrentlySelectedTile = null;
        }
    }

    protected virtual void HandleHoverOnSpace(Vector2I tile)
    {
        // No need to update anything if tile is the same
        if (tile == CurrentlySelectedTile?.Closest)
            return;

        // If there's tile selected - we need to deselect it first
        DeselectCurrentTile();

        if (!IsInPlayfield(tile, false))
            return;

        HighlightTile((tile, tile));
    }

    protected virtual void HandleHoverOnUnit(Vector2I tile, Vector2 mousePos, IPlayfieldUnit? previousUnit)
    {
        if (CurrentlySelectedUnit == null || CurrentlySelectedUnit == previousUnit)
            return;

        // If there's tile selected - we need to deselect it first
        DeselectCurrentTile();

        // Highlight tile with delta
        HighlightTile((CurrentlySelectedUnit.Coords, tile));
    }

    protected void UpdateCreaturePosition(DrawableCreatureInstance drawable)
    {
        Vector2 newPosition = MapToLocal(drawable.ParentCreature.Coords);
        if (drawable.ParentCreature.IsLargeUnit) newPosition += (Vector2)TileSet.TileSize * 0.5f;
        drawable.Position = newPosition;
    }

    protected void SetCellCustom(Vector2I coords, int sourceId)
    {
        bool isLargeTile = TileTypeExtensions.IsLargeTile(sourceId);

        if (!IsInPlayfield(coords, isLargeTile)) return;

        SetCell(coords, sourceId, Vector2I.Zero);

        if (isLargeTile)
        {
            void setCellWithOffset(Vector2I c, int sourceId, Vector2I offset) => SetCell(c + offset, sourceId, offset);

            setCellWithOffset(coords, sourceId, new Vector2I(0, 1));
            setCellWithOffset(coords, sourceId, new Vector2I(1, 0));
            setCellWithOffset(coords, sourceId, new Vector2I(1, 1));
        }
    }

    protected void ResetCell(Vector2I coords)
    {
        int sourceId = GetCellSourceId(coords);

        bool isLarge = TileTypeExtensions.IsLargeTile(sourceId);
        if (isLarge) coords -= GetCellAtlasCoords(coords);

        void resetCell(Vector2I cell, Vector2I offset)
        {
            var cellWithOffset = cell + offset;
            int sourceId = BaseSourceIds[cellWithOffset.X, cellWithOffset.Y];
            SetCell(cellWithOffset, sourceId, TileTypeExtensions.IsLargeTile(sourceId) ? offset : Vector2I.Zero);
        }

        resetCell(coords, Vector2I.Zero);

        if (isLarge)
        {
            resetCell(coords, new Vector2I(0, 1));
            resetCell(coords, new Vector2I(1, 0));
            resetCell(coords, new Vector2I(1, 1));
        }
    }

    protected void SetBaseCellCustom(Vector2I coords, int sourceId)
    {
        bool isLargeTile = TileTypeExtensions.IsLargeTile(sourceId);

        if (!IsInPlayfield(coords, isLargeTile)) return;

        void setCellWithOffset(Vector2I cell, int sourceId, Vector2I offset)
        {
            var cellWithOffset = cell + offset;
            SetCell(cellWithOffset, sourceId, offset);
            BaseSourceIds[cellWithOffset.X, cellWithOffset.Y] = sourceId;
        }

        setCellWithOffset(coords, sourceId, Vector2I.Zero);

        if (isLargeTile)
        {
            
            setCellWithOffset(coords, sourceId, new Vector2I(0, 1));
            setCellWithOffset(coords, sourceId, new Vector2I(1, 0));
            setCellWithOffset(coords, sourceId, new Vector2I(1, 1));
        }
    }

    protected DrawableCreatureInstance AddDrawableCreature(CreatureInstance creature)
    {
        var newDrawableCreature = (DrawableCreatureInstance)creature.CreateDrawableRepresentation();
        newDrawableCreature.Scale = Vector2.One * (creature.IsLargeUnit ? 4 : 2); // Asset of the tile have 2 times higher resolution than icons
        newDrawableCreature.Centered = true;
        newDrawableCreature.BackgroundSize = creature.IsLargeUnit ? 0.91 : 0.81;
        UpdateCreaturePosition(newDrawableCreature);


        Creatures.Add(newDrawableCreature);
        AddChild(newDrawableCreature);
        return newDrawableCreature;
    }

    protected abstract void HighlightTile((Vector2I Full, Vector2I Closest) tile);

    public CreatureInstance? GetPlayfieldEntityAt(Vector2I coords) 
        => Creatures.FirstOrDefault(creature => creature.ParentCreature.IsOnCoords(coords))?.ParentCreature;

    public static bool IsInPlayfield(Vector2I coords, bool isLarge)
    {
        if (isLarge)
        {
            return coords.X >= 0 && coords.Y >= 0 && coords.X < SIZE_X - 1 && coords.Y < SIZE_Y - 1;
        }
        else
        {
            return coords.X >= 0 && coords.Y >= 0 && coords.X < SIZE_X && coords.Y < SIZE_Y;
        }
    }
}

public static class TileTypeExtensions
{
    public static bool IsLargeTile(this TileType tileType) => tileType == TileType.AffectedBig || tileType == TileType.SelectBig || tileType == TileType.AimableBig;

    public static bool IsLargeTile(int sourceId) => sourceId == (int)TileType.AffectedBig || sourceId == (int)TileType.SelectBig || sourceId == (int)TileType.AimableBig;
}
