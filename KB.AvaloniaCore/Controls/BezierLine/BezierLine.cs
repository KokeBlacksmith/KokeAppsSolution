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
        AffectsGeometry<BezierLine>(StartPointProperty, EndPointProperty);
        StartPointProperty.Changed.AddClassHandler<BezierLine>(_OnPointChanged);
        EndPointProperty.Changed.AddClassHandler<BezierLine>(_OnPointChanged);
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

    private static void _OnPointChanged(BezierLine self, AvaloniaPropertyChangedEventArgs args)
    {
        
    }

    protected override Geometry? CreateDefiningGeometry()
    {
        //Point middlePoint = new Point((StartPoint.X + EndPoint.X) * 0.5d, (StartPoint.Y + EndPoint.Y) * 0.5d);
        Point middlePoint = new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(StartPoint, false);
            context.CubicBezierTo(StartPoint, middlePoint, EndPoint);
            context.EndFigure(false);
        }

        return geometry;
    }
}
