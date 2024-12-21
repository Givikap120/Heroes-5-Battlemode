using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public interface ICanMove : IPlayfieldUnit
{
    public double Speed { get; }

    public static (Vector2I Full, Vector2I Closest) ShiftMoveTileIfOccupied(Vector2I tile, bool isLarge)
    {
        if (!isLarge)
            return (tile, tile);

        static bool shift(ref Vector2I tile, Vector2I shift)
        {
            if (BattleHandler.Instance.IsTileOccupied(tile + shift))
            {
                tile -= shift;
                return true;
            }

            return false;
        }

        var shifted = tile;

        bool wasShifted = shift(ref shifted, new Vector2I(1, 0));

        if (wasShifted) shift(ref shifted, new Vector2I(0, 1));
        else
        {
            wasShifted = shift(ref shifted, new Vector2I(0, 1));

            if (wasShifted) shift(ref shifted, new Vector2I(1, 0));
            else shift(ref shifted, new Vector2I(1, 1));
        }

        return (shifted, tile);
    }

    //public Vector2I ShiftMoveTileIfTooFar(Vector2I tile, bool isLarge)
    //{
    //    if (!isLarge)
    //        return tile;

    //    double distance = this.DistanceTo(tile);

    //    if (distance > Speed)
    //    {
            
    //    }
    //}

    public bool CanMoveTo(ref Vector2I target)
    {
        if (BattleHandler.Instance.IsTileOccupied(target))
            return false;

        target = ShiftMoveTileIfOccupied(target, IsLargeUnit).Full;

        if (!CanBePlacedOnTile(target))
            return false;

        double distance = this.DistanceTo(target);
        return distance > 0 && distance <= Speed;
    }

    public bool CanMoveTo(Vector2I target) => CanMoveTo(ref target);

    public bool MoveTo(Vector2I target, bool triggerEvents = true)
    {
        if (!CanMoveTo(ref target)) return false;

        if (triggerEvents) Coords = target;
        else CoordsBindable.SetSilent(target);

        return true;
    }

    public List<Vector2I> GetPossibleMoveOptions()
    {
        List<Vector2I> result = [];

        int maxDistance = (int)(Math.Ceiling(Speed));
        int maxDistanceSqr = (int)(Speed * Speed);

        var currentTile = new Vector2I(Coords.X - maxDistance, Coords.Y - maxDistance);
        var lastTile = new Vector2I(Coords.X + maxDistance, Coords.Y + maxDistance);

        for (; currentTile.X <= lastTile.X; currentTile.X++)
        {
            for (currentTile.Y = Coords.Y - maxDistance; currentTile.Y <= lastTile.Y; currentTile.Y++)
            {
                if (CanMoveTo(currentTile))
                {
                    result.Add(currentTile);
                }
            }
        } 

        return result;
    }
}