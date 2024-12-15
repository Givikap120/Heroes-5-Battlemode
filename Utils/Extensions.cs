﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public static class EnumerableExtensions
{
    public static int MaxIndex<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        if (source == null || !source.Any())
            throw new ArgumentException("Enumerable is null or empty");

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
            throw new ArgumentException("Enumerable is null or empty");

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
}
