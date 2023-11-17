using Avalonia;
using KB.AvaloniaCore.Injection;

namespace KB.AvaloniaCore.Math;
public static class MatrixMath
{
    /// <summary>
    /// Creates a translation matrix using the specified offsets.
    /// </summary>
    /// <param name="offsetX">X-coordinate offset.</param>
    /// <param name="offsetY">Y-coordinate offset.</param>
    /// <returns>The created translation matrix.</returns>
    public static Matrix CreateTranslate(double offsetX, double offsetY)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
    }

    /// <summary>
    /// Prepends a translation around the center of provided matrix.
    /// </summary>
    /// <param name="matrix">The matrix to prepend translation.</param>
    /// <param name="offsetX">X-coordinate offset.</param>
    /// <param name="offsetY">Y-coordinate offset.</param>
    /// <returns>The created translation matrix.</returns>
    public static Matrix CreateTranslatePrepend(Matrix matrix, double offsetX, double offsetY)
    {
        return CreateTranslate(offsetX, offsetY) * matrix;
    }

    /// <summary>
    /// Creates a matrix that scales along the x-axis and y-axis.
    /// </summary>
    /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
    /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
    /// <returns>The created scaling matrix.</returns>
    public static Matrix CreateScale(double scaleX, double scaleY)
    {
        return new Matrix(scaleX, 0, 0, scaleY, 0.0, 0.0);
    }

    /// <summary>
    /// Creates a matrix that is scaling from a specified center.
    /// </summary>
    /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
    /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
    /// <param name="centerX">The center X-coordinate of the scaling.</param>
    /// <param name="centerY">The center Y-coordinate of the scaling.</param>
    /// <returns>The created scaling matrix.</returns>
    public static Matrix CreateScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }

    /// <summary>
    /// Prepends a scale around the center of provided matrix.
    /// </summary>
    /// <param name="matrix">The matrix to prepend scale.</param>
    /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
    /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
    /// <param name="centerX">The center X-coordinate of the scaling.</param>
    /// <param name="centerY">The center Y-coordinate of the scaling.</param>
    /// <returns>The created scaling matrix.</returns>
    public static Matrix CreateScaleAtPrepend(Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
    {
        return CreateScaleAt(scaleX, scaleY, centerX, centerY) * matrix;
    }

    /// <summary>
    /// Creates a translation and scale matrix using the specified offsets and scales along the x-axis and y-axis.
    /// </summary>
    /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
    /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
    /// <param name="offsetX">X-coordinate offset.</param>
    /// <param name="offsetY">Y-coordinate offset.</param>
    /// <returns>The created translation and scale matrix.</returns>
    public static Matrix CreateScaleAndTranslate(double scaleX, double scaleY, double offsetX, double offsetY)
    {
        return new Matrix(scaleX, 0.0, 0.0, scaleY, offsetX, offsetY);
    }

    /// <summary>
    /// Creates a skew matrix.
    /// </summary>
    /// <param name="angleX">Angle of skew along the X-axis in radians.</param>
    /// <param name="angleY">Angle of skew along the Y-axis in radians.</param>
    /// <returns>When the method completes, contains the created skew matrix.</returns>
    public static Matrix CreateSkew(float angleX, float angleY)
    {
        return new Matrix(1.0, System.Math.Tan(angleX), System.Math.Tan(angleY), 1.0, 0.0, 0.0);
    }

    /// <summary>
    /// Creates a matrix that rotates.
    /// </summary>
    /// <param name="radians">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis.</param>
    /// <returns>The created rotation matrix.</returns>
    public static Matrix CreateRotation(double radians)
    {
        double cos = System.Math.Cos(radians);
        double sin = System.Math.Sin(radians);
        return new Matrix(cos, sin, -sin, cos, 0, 0);
    }

    /// <summary>
    /// Creates a matrix that rotates about a specified center.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <param name="centerX">The center X-coordinate of the rotation.</param>
    /// <param name="centerY">The center Y-coordinate of the rotation.</param>
    /// <returns>The created rotation matrix.</returns>
    public static Matrix CreateRotation(double angle, double centerX, double centerY)
    {
        return CreateTranslate(-centerX, -centerY) * CreateRotation(angle) * CreateTranslate(centerX, centerY);
    }

    /// <summary>
    /// Creates a matrix that rotates about a specified center.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <param name="center">The center of the rotation.</param>
    /// <returns>The created rotation matrix.</returns>
    public static Matrix CreateRotation(double angle, Vector center)
    {
        return CreateTranslate(-center.X, -center.Y) * CreateRotation(angle) * CreateTranslate(center.X, center.Y);
    }

    /// <summary>
    /// Transforms a point by this matrix.
    /// </summary>
    /// <param name="matrix">The matrix to use as a transformation matrix.</param>
    /// <param name="point">>The original point to apply the transformation.</param>
    /// <returns>The result of the transformation for the input point.</returns>
    public static Point TransformPoint(in Matrix matrix, in Point point)
    {
        return new Point(
            (point.X * matrix.GetScaleX()) + (point.Y * matrix.GetSkewX()) + matrix.GetTranslateX(),
            (point.X * matrix.GetSkewY()) + (point.Y * matrix.GetScaleY()) + matrix.GetTranslateY());
    }

    public static Matrix CloneAndSetScaleX(in Matrix source, in double scaleX)
    {
        return new Matrix(scaleX,
                          source.GetSkewY(),
                          source.GetSkewX(),
                          source.GetScaleY(),
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetScaleY(in Matrix source, in double scaleY)
    {
        return new Matrix(source.GetScaleX(),
                          source.GetSkewY(),
                          source.GetSkewX(),
                          scaleY,
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetSkewX(in Matrix source, in double skewX)
    {
        return new Matrix(source.GetScaleX(),
                          source.GetSkewY(),
                          skewX,
                          source.GetScaleY(),
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetSkewY(in Matrix source, in double skewY)
    {
        return new Matrix(source.GetScaleX(),
                          skewY,
                          source.GetSkewX(),
                          source.GetScaleY(),
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetTranslateX(in Matrix source, in double translateX)
    {
        return new Matrix(source.GetScaleX(),
                          source.GetSkewY(),
                          source.GetSkewX(),
                          source.GetScaleY(),
                          translateX,
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetTranslateY(in Matrix source, in double translateY)
    {
        return new Matrix(source.GetScaleX(),
                          source.GetSkewY(),
                          source.GetSkewX(),
                          source.GetScaleY(),
                          source.GetTranslateX(),
                          translateY);
    }

    public static Matrix CloneAndSetScale(in Matrix source, in double scaleX, in double scaleY)
    {
        return new Matrix(scaleX,
                          source.GetSkewY(),
                          source.GetSkewX(),
                          scaleY,
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetSkew(in Matrix source, in double skewX, in double skewY)
    {
        return new Matrix(source.GetScaleX(),
                          skewY,
                          skewX,
                          source.GetScaleY(),
                          source.GetTranslateX(),
                          source.GetTranslateY());
    }

    public static Matrix CloneAndSetTranslate(in Matrix source, in double translateX, in double translateY)
    {
        return new Matrix(source.GetScaleX(),
                          source.GetSkewY(),
                          source.GetSkewX(),
                          source.GetScaleY(),
                          translateX,
                          translateY);
    }
}
