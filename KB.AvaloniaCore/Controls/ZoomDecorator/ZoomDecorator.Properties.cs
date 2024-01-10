using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Transformation;
using Avalonia;
using Avalonia.Media;
using KB.AvaloniaCore.Controls.Events;
using KB.SharpCore.Synchronization;

namespace KB.AvaloniaCore.Controls;

public partial class ZoomDecorator
{
    /// <summary>
    /// Identifies the <seealso cref="ZoomSpeed"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomSpeedProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="PowerFactor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> PowerFactorProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(PowerFactor), 1, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="TransitionThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> TransitionThresholdProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(TransitionThreshold), 0.5, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="Stretch"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchModeProperty =
        AvaloniaProperty.Register<ZoomDecorator, Stretch>(nameof(StretchMode), Stretch.Uniform, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="ZoomX"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomDecorator, double> ZoomXProperty =
        AvaloniaProperty.RegisterDirect<ZoomDecorator, double>(nameof(ZoomX), o => o.ZoomX, null, 1.0);

    /// <summary>
    /// Identifies the <seealso cref="ZoomY"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomDecorator, double> ZoomYProperty =
        AvaloniaProperty.RegisterDirect<ZoomDecorator, double>(nameof(ZoomY), o => o.ZoomY, null, 1.0);

    /// <summary>
    /// Identifies the <seealso cref="OffsetX"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomDecorator, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<ZoomDecorator, double>(nameof(OffsetX), o => o.OffsetX, null, 0.0);

    /// <summary>
    /// Identifies the <seealso cref="OffsetY"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomDecorator, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<ZoomDecorator, double>(nameof(OffsetY), o => o.OffsetY, null, 0.0);

    /// <summary>
    /// Identifies the <seealso cref="EnableConstrains"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableConstrainsProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(EnableConstrains), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinZoomX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomXProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MinZoomX), Double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxZoomX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomXProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MaxZoomX), Double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinZoomY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomYProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MinZoomY), Double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxZoomY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomYProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MaxZoomY), Double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinOffsetX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinOffsetXProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MinOffsetX), Double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxOffsetX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxOffsetXProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MaxOffsetX), Double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinOffsetY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinOffsetYProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MinOffsetY), Double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxOffsetY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxOffsetYProperty =
        AvaloniaProperty.Register<ZoomDecorator, double>(nameof(MaxOffsetY), Double.PositiveInfinity, false, BindingMode.TwoWay);

    public static readonly StyledProperty<bool> ConstraintOffsetByParentBoundsProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(ConstraintOffsetByParentBounds), false, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnablePan"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnablePanProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(EnablePan), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableZoom"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableZoomProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(EnableZoom), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="IsZoomEnabled"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsZoomEnabledProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(IsZoomEnabled), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="IsPanEnabled"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPanEnabledProperty =
        AvaloniaProperty.Register<ZoomDecorator, bool>(nameof(IsPanEnabled), true, false, BindingMode.TwoWay);

    static ZoomDecorator()
    {
        AffectsArrange<ZoomDecorator>(
            ZoomSpeedProperty,
            StretchModeProperty,
            EnableConstrainsProperty,
            ConstraintOffsetByParentBoundsProperty,
            MinZoomXProperty,
            MaxZoomXProperty,
            MinZoomYProperty,
            MaxZoomYProperty,
            MinOffsetXProperty,
            MaxOffsetXProperty,
            MinOffsetYProperty,
            MaxOffsetYProperty
            );

        Decorator.ChildProperty.Changed.AddClassHandler<ZoomDecorator>((s, e) => s.ChildChanged(e.GetNewValue<Control?>()));
        Decorator.BoundsProperty.Changed.AddClassHandler<ZoomDecorator>((s, e) => s.BoundsChanged(e.GetNewValue<Rect>()));
    }

    private Control? _child;
    private Point _pan;
    private Point _previous;
    private Matrix _matrix;
    private TransformOperations.Builder _transformBuilder;
    private bool _isPanning;
    private readonly BooleanRAIIOperation _updating = new BooleanRAIIOperation();
    private double _zoomX = 1.0;
    private double _zoomY = 1.0;
    private double _offsetX = 0.0;
    private double _offsetY = 0.0;
    private bool _captured = false;


    /// <summary>
    /// Zoom changed event.
    /// </summary>
    public event ZoomChangedEventHandler? ZoomChanged;

    /// <summary>
    /// Gets or sets zoom speed ratio.
    /// </summary>
    public double ZoomSpeed
    {
        get => GetValue(ZoomSpeedProperty);
        set => SetValue(ZoomSpeedProperty, value);
    }

    /// <summary>
    /// Gets or sets the power factor used to transform the mouse wheel delta value.
    /// </summary>
    public double PowerFactor
    {
        get => GetValue(PowerFactorProperty);
        set => SetValue(PowerFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the threshold below which zoom operations will skip all transitions.
    /// </summary>
    public double TransitionThreshold
    {
        get => GetValue(TransitionThresholdProperty);
        set => SetValue(TransitionThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets stretch mode.
    /// </summary>
    public Stretch StretchMode
    {
        get => GetValue(StretchModeProperty);
        set => SetValue(StretchModeProperty, value);
    }

    /// <summary>
    /// Gets the render transform matrix.
    /// </summary>
    public Matrix Matrix => _matrix;

    /// <summary>
    /// Gets the zoom ratio for x axis.
    /// </summary>
    public double ZoomX => _zoomX;

    /// <summary>
    /// Gets the zoom ratio for y axis.
    /// </summary>
    public double ZoomY => _zoomY;

    /// <summary>
    /// Gets the pan offset for x axis.
    /// </summary>
    public double OffsetX => _offsetX;

    /// <summary>
    /// Gets the pan offset for y axis.
    /// </summary>
    public double OffsetY => _offsetY;

    /// <summary>
    /// Gets or sets flag indicating whether zoom ratio and pan offset constrains are applied.
    /// </summary>
    public bool EnableConstrains
    {
        get => GetValue(EnableConstrainsProperty);
        set => SetValue(EnableConstrainsProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum zoom ratio for x axis.
    /// </summary>
    public double MinZoomX
    {
        get => GetValue(MinZoomXProperty);
        set => SetValue(MinZoomXProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum zoom ratio for x axis.
    /// </summary>
    public double MaxZoomX
    {
        get => GetValue(MaxZoomXProperty);
        set => SetValue(MaxZoomXProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum zoom ratio for y axis.
    /// </summary>
    public double MinZoomY
    {
        get => GetValue(MinZoomYProperty);
        set => SetValue(MinZoomYProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum zoom ratio for y axis.
    /// </summary>
    public double MaxZoomY
    {
        get => GetValue(MaxZoomYProperty);
        set => SetValue(MaxZoomYProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum offset for x axis.
    /// </summary>
    public double MinOffsetX
    {
        get => GetValue(MinOffsetXProperty);
        set => SetValue(MinOffsetXProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum offset for x axis.
    /// </summary>
    public double MaxOffsetX
    {
        get => GetValue(MaxOffsetXProperty);
        set => SetValue(MaxOffsetXProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum offset for y axis.
    /// </summary>
    public double MinOffsetY
    {
        get => GetValue(MinOffsetYProperty);
        set => SetValue(MinOffsetYProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum offset for y axis.
    /// </summary>
    public double MaxOffsetY
    {
        get => GetValue(MaxOffsetYProperty);
        set => SetValue(MaxOffsetYProperty, value);
    }

    public bool ConstraintOffsetByParentBounds
    {
        get => GetValue(ConstraintOffsetByParentBoundsProperty);
        set => SetValue(ConstraintOffsetByParentBoundsProperty, value);
    }

    /// <summary>
    /// Gets or sets flag indicating whether pan input events are processed.
    /// </summary>
    public bool EnablePan
    {
        get => GetValue(EnablePanProperty);
        set => SetValue(EnablePanProperty, value);
    }

    /// <summary>
    /// Gets or sets flag indicating whether input zoom events are processed.
    /// </summary>
    public bool EnableZoom
    {
        get => GetValue(EnableZoomProperty);
        set => SetValue(EnableZoomProperty, value);
    }

    public bool IsZoomEnabled
    {
        get => GetValue(IsZoomEnabledProperty);
        set => SetValue(IsZoomEnabledProperty, value);
    }

    public bool IsPanEnabled
    {
        get => GetValue(IsPanEnabledProperty);
        set => SetValue(IsPanEnabledProperty, value);
    }
}
