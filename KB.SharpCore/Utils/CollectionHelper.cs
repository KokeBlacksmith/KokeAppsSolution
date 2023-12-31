﻿using System.Collections;
using System.Linq;

namespace KB.SharpCore.Utils;

public static class CollectionHelper
{
    public static bool IsEmpty(ICollection? enumerable)
    {
        return enumerable == null || enumerable.Count == 0;
    }
    
    public static bool HasAny(ICollection? enumerable)
    {
        return !CollectionHelper.IsEmpty(enumerable);
    }

    public static bool IsEmpty<T>(IEnumerable<T>? enumerable)
    {
        return enumerable == null || !enumerable.Any();
    }

    public static bool HasAny<T>(IEnumerable<T>? enumerable)
    {
        return !CollectionHelper.IsEmpty(enumerable);
    }

    public static void Dispose<T>(IEnumerable<T>? enumerable)
    {
        foreach (T item in enumerable ?? Enumerable.Empty<T>())
        {
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Returns a single string with each element of the array on a new line.
    /// </summary>
    /// <param name="array">Array of strings</param>
    /// <returns></returns>
    public static string? StringEnumerableToNewLinesString(IEnumerable<string>? array)
    {
        if (CollectionHelper.HasAny(array))
        {
            return String.Join(Environment.NewLine, array!);
        }
        else
        {
            return null;
        }
    }

    public static int IndexOf<T>(IEnumerable<T> enumerable, T itemToFind)
    {
        var index = 0;
        foreach (T item in enumerable)
        {
            if (item?.Equals(itemToFind) ?? false)
            {
                return index;
            }

            index++;
        }

        return -1;
    }
}