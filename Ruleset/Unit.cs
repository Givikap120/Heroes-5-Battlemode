/// <summary>
///	Entity on the battlefield that can do action and participates in Initiative cycle
/// </summary>
public abstract class Unit
{
    public readonly Player Player;

    public Unit(Player player)
    { 
        Player = player; 
    }

    public abstract DrawableCreatureInstance CreateDrawableRepresentation();

    /// <summary>
    /// Value between 0 and 1 that determines how far is turn of this Unit
    /// </summary>
    public double ATB = double.NaN;

    public abstract double Initiative { get; }

    /// <summary>
    /// Tells UI how to change the tile depending on previous tile type. Returns -1 if tile won't be changed to a specific type
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public abstract int DecideTileChange(int tileType);
}

public static class UnitExtensions
{
    public static bool IsAlly(this Unit a, Unit b) => a.Player.Id == b.Player.Id;
}