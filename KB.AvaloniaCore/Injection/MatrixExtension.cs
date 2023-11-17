using Avalonia;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace KB.AvaloniaCore.Injection;
public static class MatrixExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetScaleX(this Matrix matrix)
    {
        return matrix.M11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetScaleY(this Matrix matrix)
    {
        return matrix.M22;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetSkewX(this Matrix matrix)
    {
        return matrix.M21;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetSkewY(this Matrix matrix)
    {
        return matrix.M12;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetTranslateX(this Matrix matrix)
    {
        return matrix.M31;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetTranslateY(this Matrix matrix)
    {
        return matrix.M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetScale(this Matrix matrix)
    {
        return new Vector2((float)matrix.GetScaleX(), (float)matrix.GetScaleY());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetSkew(this Matrix matrix)
    {
        return new Vector2((float)matrix.GetSkewX(), (float)matrix.GetSkewY());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetTranslate(this Matrix matrix)
    {
        return new Vector2((float)matrix.GetTranslateX(), (float)matrix.GetTranslateY());
    }
}
