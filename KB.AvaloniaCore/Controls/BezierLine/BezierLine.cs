using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;

namespace KB.AvaloniaCore.Controls;
public class BezierLine : Shape
{
    static BezierLine()
    {
        StrokeThicknessProperty.OverrideDefaultValue<BezierLine>(1);
        AffectsGeometry<BezierLine>(StartPointProperty, EndPointProperty, StartAngleProperty, EndAngleProperty, ControlPointDistanceProperty);
    }

    public BezierLine()
    {
    }

    /// <summary>
    /// Defines the <see cref="StartPoint"/> property.
    /// </summary>
    public static readonly StyledProperty<Point> StartPointProperty
                    = AvaloniaProperty.Register<BezierLine, Point>(nameof(StartPoint));
    /// <summary>
    /// Defines the <see cref="EndPoint"/> property.
    /// </summary>
    public static readonly StyledProperty<Point> EndPointProperty
                    = AvaloniaProperty.Register<BezierLine, Point>(nameof(EndPoint));

    public static readonly StyledProperty<double?> StartAngleProperty
                    = AvaloniaProperty.Register<BezierLine, double?>(nameof(StartAngle), 0.0d);

    public static readonly StyledProperty<double?> EndAngleProperty
                    = AvaloniaProperty.Register<BezierLine, double?>(nameof(EndAngle), 0.0d);

    public static readonly StyledProperty<double> ControlPointDistanceProperty
                    = AvaloniaProperty.Register<BezierLine, double>(nameof(ControlPointDistance), 50d);

    /// <summary>
    /// Gets or sets the <see cref="StartPoint"/>.
    /// </summary>
    /// <value>
    /// The <see cref="StartPoint"/>.
    /// </value>
    public Point StartPoint
    {
        get { return GetValue(StartPointProperty); }
        set { SetValue(StartPointProperty, value); }
    }

    /// <summary>
    /// Gets or sets the <see cref="EndPoint"/>.
    /// </summary>
    /// <value>
    /// The <see cref="EndPoint"/>.
    /// </value>
    public Point EndPoint
    {
        get { return GetValue(EndPointProperty); }
        set { SetValue(EndPointProperty, value); }
    }

    /// <summary>
    /// Start angle of the line in degrees.
    /// </summary>
    public double? StartAngle
    {
        get { return GetValue(StartAngleProperty); }
        set { SetValue(StartAngleProperty, value); }
    }

    /// <summary>
    /// End angle of the line in degrees.
    /// </summary>
    public double? EndAngle
    {
        get { return GetValue(EndAngleProperty); }
        set { SetValue(EndAngleProperty, value); }
    }

    /// <summary>
    /// Distance of the control points from the start and end points.
    /// </summary>
    public double ControlPointDistance
    {
        get { return GetValue(ControlPointDistanceProperty); }
        set { SetValue(ControlPointDistanceProperty, value); }
    }

    protected override Geometry? CreateDefiningGeometry()
    {
        StreamGeometry geometry = new StreamGeometry();
        using (StreamGeometryContext context = geometry.Open())
        {
            // Calculate control points for Bezier curve
            Point controlPoint1;
            if(!StartAngle.HasValue)
            {
                controlPoint1 = StartPoint;
            }
            else
            {
                double startAngleRad = KB.SharpCore.Utils.Math.DegToRad(StartAngle.Value);
                controlPoint1 = new Point(
                    StartPoint.X + System.Math.Cos(startAngleRad) * ControlPointDistance,
                    StartPoint.Y + System.Math.Sin(startAngleRad) * ControlPointDistance);
            }

            Point controlPoint2;
            if (!EndAngle.HasValue)
            {
                controlPoint2 = EndPoint;
            }
            else
            {
                double endAngleRad = KB.SharpCore.Utils.Math.DegToRad(EndAngle.Value);
                controlPoint2 = new Point(
                    EndPoint.X + System.Math.Cos(endAngleRad) * ControlPointDistance,
                    EndPoint.Y + System.Math.Sin(endAngleRad) * ControlPointDistance);
            }

            // Start the figure
            context.BeginFigure(StartPoint, isFilled: false);

            // Add Bezier segment
            context.CubicBezierTo(controlPoint1, controlPoint2, EndPoint);
            context.EndFigure(false);
        }

        return geometry;
    }
}
