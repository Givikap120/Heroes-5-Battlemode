﻿using Godot;
using System;
using System.Collections.Generic;
using static Playfield;

public interface ICanMove : IPlayfieldUnit
{
    public double Speed { get; }

    public bool CanMoveTo(Vector2I target)
    {
        double maxDistance = Speed * Speed;
        double distanceSquared = target.DistanceSquaredTo(Coords);
        return distanceSquared > 0 && distanceSquared <= maxDistance;
    }

    public bool MoveTo(Vector2I target)
    {
        if (!CanMoveTo(target)) return false;
        Coords = target;
        return true;
    }

    public List<Vector2I> GetPossibleMoveOptions(Func<Vector2I, bool> isOccupied)
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
                if (!IsInPlayfield(currentTile) || isOccupied(currentTile))
                    continue;

                int distanceSqr = currentTile.DistanceSquaredTo(Coords);
                if (distanceSqr <= maxDistanceSqr && distanceSqr > 0)
                {
                    result.Add(currentTile);
                }
            }
        } 

        return result;
    }
}