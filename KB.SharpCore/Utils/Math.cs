using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KB.SharpCore.Utils;

public static class Math
{
    public const double PI = System.Math.PI;
    public const double PI2 = System.Math.PI * 2.0d;
    public const double PIHalf = System.Math.PI * 0.5d;
    public const double PIQuarter = System.Math.PI * 0.25d;


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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetRadAngleBetweenPoints(double p1X, double p1Y, double p2X, double p2Y)
    {
        double xDiff = p2X - p1X;
        double yDiff = p2Y - p1Y;

        // Use Math.Atan2 to get the angle in radians
        double radians = System.Math.Atan2(yDiff, xDiff);

        // Ensure the angle is positive
        double positiveRadians = (radians + Math.PI2) % Math.PI2;
        return positiveRadians;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetRadAngleBetweenPoints(Point p1, Point p2)
    {
        return GetRadAngleBetweenPoints(p1.X, p1.Y, p2.X, p2.Y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetDegAngleBetweenPoints(Point p1, Point p2)
    {
        return RadToDeg(GetRadAngleBetweenPoints(p1, p2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetDegAngleBetweenPoints(double p1X, double p1Y, double p2X, double p2Y)
    {
        return RadToDeg(GetRadAngleBetweenPoints(p1X, p1Y, p2X, p2Y));
    }
}
