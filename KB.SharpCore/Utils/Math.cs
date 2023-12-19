using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KB.SharpCore.Utils;

public static class Math
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween<T>(this T item, in T start, in T end)
        where T : IComparable, IComparable<T>
    {
        return Comparer<T>.Default.Compare(item, start) >= 0 && Comparer<T>.Default.Compare(item, end) <= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double DegToRad(in double deg)
    {
        return deg * System.Math.PI / 180.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RadToDeg(in double rad)
    {
        return rad * 180.0 / System.Math.PI;
    }
}
