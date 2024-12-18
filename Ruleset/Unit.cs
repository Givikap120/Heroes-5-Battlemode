
using Godot;

/// <summary>
///	Entity on the battlefield that can do action and participates in Initiative cycle
/// </summary>
public abstract partial class Unit : GodotObject, IUnit
{
    public Player Player { get; private set; }

    public Unit(Player player)
    { 
        Player = player; 
    }

    public abstract DrawableCreatureInstance CreateDrawableRepresentation();

    public const double BASE_INITIATIVE = 10;

    /// <summary>
    /// Value between 0 and 1 that determines how far is turn of this Unit
    /// </summary>
    public double ATB { get; set; } = double.NaN;

    public abstract double Initiative { get; }

    /// <summary>
    /// Tells UI how to change the tile depending on previous tile type. Returns -1 if tile won't be changed to a specific type
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public abstract int DecideTileChange(int tileType);

    public abstract UnitState SaveState();
    public abstract void LoadState(UnitState savedState, bool silent = true);
}

public static class UnitExtensions
{
    public static bool IsAlly(this IUnit a, IUnit b) => a.Player.Id == b.Player.Id;

    public static double GetRemainingTurnsToMove(this Unit unit) => (1 - unit.ATB) * Unit.BASE_INITIATIVE / unit.Initiative;
}