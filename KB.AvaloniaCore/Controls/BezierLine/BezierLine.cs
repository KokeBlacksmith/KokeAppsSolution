using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;

namespace KB.AvaloniaCore.Controls;
public class BezierLine : Shape
{
    private readonly BezierSegment _bezierSegment;
    private readonly PathFigures _figures;
    private readonly PathGeometry geometry;

    static BezierLine()
    {
        StrokeThicknessProperty.OverrideDefaultValue<BezierLine>(1);
        AffectsGeometry<BezierLine>(StartPointProperty, EndPointProperty);
        StartPointProperty.Changed.AddClassHandler<BezierLine>(_OnPointChanged);
        EndPointProperty.Changed.AddClassHandler<BezierLine>(_OnPointChanged);
    }

    public BezierLine()
    {
        geometry = new PathGeometry();
        _bezierSegment = new BezierSegment();
        _figures = new PathFigures
        {
            new PathFigure
            {
                Segments = new PathSegments
                {
                    _bezierSegment
                },
                IsFilled = false,
                IsClosed = false
            }
        };

        geometry.Figures = _figures;
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
        //Point middlePoint = new Point(self.EndPoint.X - self.StartPoint.X, self.EndPoint.Y - self.StartPoint.Y);
        Point middlePoint = new Point((self.StartPoint.X + self.EndPoint.X) * 0.5d, (self.StartPoint.Y + self.EndPoint.Y) * 0.5d);
        self._figures[0].StartPoint = self.StartPoint;
        self._bezierSegment.Point1 = self.StartPoint;
        self._bezierSegment.Point2 = middlePoint;
        self._bezierSegment.Point3 = self.EndPoint;
    }

    protected override Geometry? CreateDefiningGeometry()
    {
        return geometry;
    }
}
