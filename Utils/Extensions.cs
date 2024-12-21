using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public static class EnumerableExtensions
{
    public static int MaxIndex<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        if (source == null || !source.Any())
            return -1;

        int maxIndex = 0;
        int currentIndex = 0;
        TSource maxValue = source.First();

        foreach (TSource value in source)
        {
            if (value.CompareTo(maxValue) > 0)
            {
                maxValue = value;
                maxIndex = currentIndex;
            }
            currentIndex++;
        }

        return maxIndex;
    }

    public static int MinIndex<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        if (source == null || !source.Any())
            return -1;

        int minIndex = 0;
        int currentIndex = 0;
        TSource minValue = source.First();

        foreach (TSource value in source)
        {
            if (value.CompareTo(minValue) < 0)
            {
                minValue = value;
                minIndex = currentIndex;
            }
            currentIndex++;
        }

        return minIndex;
    }

    public static int FirstIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> pred)
    {
        if (source == null || !source.Any())
            return -1;

        int currentIndex = 0;
        foreach (TSource value in source)
        {
            if (pred(value))
                return currentIndex;

            currentIndex++;
        }

        return -1;
    }
}

public static class CoordExtensions
{
    public static bool IsNeighboring(this Vector2I cell, Vector2I other)
    {
        Vector2I delta = (cell - other).Abs();
        return Math.Max(delta.X, delta.Y) == 1;
    }

    public static Vector2I[] GetNeighboring(this Vector2I cell)
    {
        Vector2I[] neighbors = new Vector2I[8];

        int index = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                neighbors[index] = new Vector2I(cell.X + i, cell.Y - j);
                index++;
            }
        }

        return neighbors;
    }
    public static Vector2I[] GetSquare(Vector2I offset) => [
        offset,
        offset + new Vector2I(1, 0),
        offset + new Vector2I(0, 1),
        offset + new Vector2I(1, 1)
    ];

    public static Vector2I[] GetPartialSquare(Vector2I offset) => [
        offset + new Vector2I(1, 0),
        offset + new Vector2I(0, 1),
        offset + new Vector2I(1, 1)
    ];
}
