
using Godot;
using static Playfield;

/// <summary>
///	Entity on the battlefield that can do action and participates in Initiative cycle
/// </summary>
public abstract partial class Unit : GodotObject, IUnit
{
    public abstract string Name { get; }

    public Player Player { get; private set; }

    public Unit(Player player)
    { 
        Player = player; 
    }

    public abstract string IconPath { get; set; }
    public abstract DrawableUnit CreateDrawableRepresentation();

    public const double BASE_INITIATIVE = 10;

    /// <summary>
    /// Value between 0 and 1 that determines how far is turn of this Unit
    /// </summary>
    public double ATB { get; set; } = double.NaN;

    public abstract double Initiative { get; }

    /// <summary>
    /// Determines what happens when defend action is used.
    /// By default - nothing happens.
    /// </summary>
    public virtual void Defend() { }

    public virtual bool IsLargeUnit => false;

    /// <summary>
    /// Tells UI how to change the tile depending on previous tile type. Returns -1 if tile won't be changed to a specific type
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public virtual int DecideTileChange(int tileType)
    {
        if (tileType == (int)TileType.Affected)
            return IsLargeUnit ? (int)TileType.SelectBig : (int)TileType.Select;

        if (tileType == (int)TileType.Aimable)
            return (int)TileType.Select;

        if (tileType == (int)TileType.AffectedBig || tileType == (int)TileType.AimableBig)
            return (int)TileType.SelectBig;

        return -1;
    }

    public abstract UnitState SaveState();
    public abstract void LoadState(UnitState savedState, bool silent = true);
}

public static class UnitExtensions
{
    public static bool IsAlly(this IUnit a, IUnit b) => a.Player.Id == b.Player.Id;

    public static double GetRemainingTurnsToMove(this Unit unit) => (1 - unit.ATB) * Unit.BASE_INITIATIVE / unit.Initiative;
}