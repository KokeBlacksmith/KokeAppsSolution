using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using KB.AvaloniaCore.Injection;

namespace KB.AvaloniaCore.Controls.GraphEditor;

/// <summary>
/// Wire that connects two node pins
/// </summary>
public class NodeConnection : Control
{
    /// <summary>
    /// Connection drawing
    /// </summary>
    private readonly BezierLine _line;

    static NodeConnection()
    {
        SourcePinProperty.Changed.AddClassHandler<NodeConnection>(_OnSourcePinPropertyChanged);
        TargetPinProperty.Changed.AddClassHandler<NodeConnection>(_OnTargetPinPropertyChanged);
    }

    // no solo se le puede pasar el source y el target, también podemos crear una conexiópn sin haber finalizado la edición,
    // por lo que el Source siempre será un NodePin pero el target puede ser un NodePin o un Point
    public NodeConnection(NodePin source, NodePin target, Point sourcePosition, Point targetPosition) : this(sourcePosition, targetPosition)
    {
        SourcePin = source ?? throw new ArgumentNullException(nameof(source));
        TargetPin = target ?? throw new ArgumentNullException(nameof(target));
    }

    public NodeConnection(NodePin source, Point sourcePosition, Point targetPosition) : this(sourcePosition, targetPosition)
    {
        SourcePin = source ?? throw new ArgumentNullException(nameof(source));
    }

    private NodeConnection(Point sourcePosition, Point targetPosition)
    {
        _line = new BezierLine();
        LogicalChildren.Add(_line);
        VisualChildren.Add(_line);

        // Set start and end point// Set start and end point
        _line.StartPoint = sourcePosition;
        _line.EndPoint = targetPosition;

        //TODO: remove this, just for testing
        // Color has to be in styled properties
        _line.Stroke = Brushes.Yellow;
        _line.StrokeThickness = 2;
    }

    public static readonly StyledProperty<NodePin?> SourcePinProperty = AvaloniaProperty.Register<NodeConnection, NodePin?>(nameof(NodeConnection.SourcePin));
    public static readonly StyledProperty<NodePin?> TargetPinProperty = AvaloniaProperty.Register<NodeConnection, NodePin?>(nameof(NodeConnection.TargetPin));
    
    public NodePin? SourcePin
    {
        get { return GetValue(NodeConnection.SourcePinProperty); }
        set { SetValue(NodeConnection.SourcePinProperty, value); }
    }
    
    public NodePin? TargetPin
    {
        get { return GetValue(NodeConnection.TargetPinProperty); }
        set { SetValue(NodeConnection.TargetPinProperty, value); }
    }

    public void UpdateStartPoint(Point point)
    {
        if (TargetPin is null)
        {
            throw new InvalidOperationException("Target can't be null on updating connection start point.");
        }

        SourcePin = null;
        _line.StartPoint = point;
    }
    public void UpdateEndPoint(Point point)
    {
        if(SourcePin is null)
        {
            throw new InvalidOperationException("Source can't be null on updating connection end point.");
        }

        TargetPin = null;
        _line.EndPoint = point;
    }

    /// <summary>
    /// Sets the source or target pin, depending which one is missing to set.
    /// A connection always has to have a source or a target pin set at least.
    /// </summary>
    /// <param name="pin"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetMissingPin(NodePin pin)
    {
        if (SourcePin is not null && TargetPin is not null)
        {
            throw new InvalidOperationException("Source and Target pin are connected. Can't update end point.");
        }

        if(SourcePin is null)
        {
            SourcePin = pin;
        }
        else
        {
            TargetPin = pin;
        }
    }

    public void DisconnectPin(NodePin pin)
    {
        if(SourcePin == pin)
        {
            SourcePin = null;
        }
        else if(TargetPin == pin)
        {
            TargetPin = null;
        }
    }

    public void DisconnectPin(NodePin pin, Point connectionPoint)
    {
        if (SourcePin == pin)
        {
            UpdateStartPoint(connectionPoint);
        }
        else if (TargetPin == pin)
        {
            UpdateEndPoint(connectionPoint);
        }
    }

    private static void _OnSourcePinPropertyChanged(NodeConnection self, AvaloniaPropertyChangedEventArgs args)
    {
        if(args.OldValue is NodePin oldPin)
        {
            oldPin.IsConnected = false;
        }

        if(args.NewValue is NodePin newPin)
        {
            newPin.IsConnected = true;
            self._line.StartPoint = self._GePinCenterPosition(newPin);
        }
        else if (self.TargetPin is null)
        {
            throw new InvalidOperationException("TargetPin is null. At least one pin has to be connected.");
        }
    }

    private static void _OnTargetPinPropertyChanged(NodeConnection self, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.OldValue is NodePin oldPin)
        {
            oldPin.IsConnected = false;
        }

        if (args.NewValue is NodePin newPin)
        {
            newPin.IsConnected = true;
            self._line.EndPoint = self._GePinCenterPosition(newPin);
        }
        else if (self.SourcePin is null)
        {
            throw new InvalidOperationException("SourcePin is null. At least one pin has to be connected.");
        }
    }

    private Point _GePinCenterPosition(NodePin pin)
    {
        Point pinPosition = CanvasExtension.GetCanvasControlCenter(pin);
        Canvas parentNodeCanvas = pin.ParentNode!.GetParentOfType<Canvas>();
        return pin.ParentNode!.TranslatePoint(pinPosition, parentNodeCanvas)!.Value;
    }
}
