using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetDistanceBetweenPoints(Point p1, Point p2)
    {
        return GetDistanceBetweenPoints(p1.X, p1.Y, p2.X, p2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetDistanceBetweenPoints(double x1, double y1, double x2, double y2)
    {
        double xDiff = x2 - x1;
        double yDiff = y2 - y1;

        return System.Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <summary>
    /// Returns from which side the point is located relative to the center.
    /// </summary>
    /// <param name="center">Center position</param>
    /// <param name="point">Position of the point relative to the center</param>
    /// <returns>
    /// 0 = Top <br/>
    /// 1 = Left <br/>
    /// 2 = Bottom <br/>
    /// 3 = Right
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Get2DSideFromCenterToPoint(Point center, Point point)
    {
        return Get2DSideFromCenterToPoint(center.X, center.Y, point.X, point.Y);
    }

    /// <summary>
    /// Returns from which side the point is located relative to the center.
    /// </summary>
    /// <returns>
    /// 0 = Top <br/>
    /// 1 = Left <br/>
    /// 2 = Bottom <br/>
    /// 3 = Right
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Get2DSideFromCenterToPoint(double centerX, double centerY, double pointX, double pointY)
    {
        // Get the angle between the points
        double angle = KB.SharpCore.Utils.Math.GetRadAngleBetweenPoints(centerX, centerY, pointX, pointY);

        // Check where the pin is located in the node. It can be top, bottom, left or right.
        // Depending on that, we have to set the angle of the connection.
        if (angle.IsBetween(KB.SharpCore.Utils.Math.PIQuarter, (3 * System.Math.PI) / 4.0d))
        {
            //Top
            return 0;
        }
        else if (angle.IsBetween((3 * System.Math.PI) / 4.0d, (5 * System.Math.PI) / 4.0d))
        {
            //Left
            return 1;
        }
        else if (angle.IsBetween((5 * System.Math.PI) / 4.0d, (7 * System.Math.PI) / 4.0d))
        {
            //Bottom
            return 2;
        }
        else
        {
            //Right
            return 3;
        }
    }

}
