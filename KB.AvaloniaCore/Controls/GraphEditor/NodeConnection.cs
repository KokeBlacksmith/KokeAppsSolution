using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using KB.AvaloniaCore.Controls.GraphEditor.Events;
using KB.AvaloniaCore.Injection;
using KB.SharpCore.Events;
using KB.SharpCore.Utils;

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

    /// <summary>
    /// Invisible line that is used to detect clicks on the connection easier
    /// </summary>
    private readonly BezierLine _invisibleClickLine;

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
        _invisibleClickLine = new BezierLine();
        LogicalChildren.Add(_line);
        VisualChildren.Add(_line);
        VisualChildren.Add(_invisibleClickLine);

        _invisibleClickLine[!BezierLine.StartPointProperty] = _line[!BezierLine.StartPointProperty];
        _invisibleClickLine[!BezierLine.EndPointProperty] = _line[!BezierLine.EndPointProperty];
        _invisibleClickLine[!BezierLine.StartAngleProperty] = _line[!BezierLine.StartAngleProperty];
        _invisibleClickLine[!BezierLine.EndAngleProperty] = _line[!BezierLine.EndAngleProperty];
        _invisibleClickLine[!BezierLine.ControlPointDistanceProperty] = _line[!BezierLine.ControlPointDistanceProperty];
        _invisibleClickLine.Stroke = Brushes.Transparent;

        // Set start and end point// Set start and end point
        _line.StartPoint = sourcePosition;
        _line.EndPoint = targetPosition;

        //TODO: remove this, just for testing
        // Color has to be in styled properties
        _line.Stroke = Brushes.Yellow;
        _line.StrokeThickness = 2;
        _invisibleClickLine.StrokeThickness = 10;
    }

    public new event EventHandler<NodeConnectionPointerInteractionEventArgs>? PointerPressed;
    public new event EventHandler<NodeConnectionPointerInteractionEventArgs>? PointerMoved;
    public new event EventHandler<NodeConnectionPointerInteractionEventArgs>? PointerReleased;

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
        _line.StartAngle = null;
        _line.StartPoint = point;
    }

    public void UpdateEndPoint(Point point)
    {
        if(SourcePin is null)
        {
            throw new InvalidOperationException("Source can't be null on updating connection end point.");
        }

        TargetPin = null;
        _line.EndAngle = null;
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
            oldPin.ParentNode!.PositionXChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.PositionYChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.WidthChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.HeightChanged -= self._OnNodePositionChanged;
        }

        if(args.NewValue is NodePin newPin)
        {
            newPin.IsConnected = true;
            newPin.ParentNode!.PositionXChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.PositionYChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.WidthChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.HeightChanged += self._OnNodePositionChanged;
        }
        else if (self.TargetPin is null)
        {
            throw new InvalidOperationException("TargetPin is null. At least one pin has to be connected.");
        }
        else
        {
            self._line.StartAngle = null;
        }

        self._UpdateLine();
        self._UpdateConnectionZIndex();
    }

    private static void _OnTargetPinPropertyChanged(NodeConnection self, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.OldValue is NodePin oldPin)
        {
            oldPin.IsConnected = false;
            oldPin.ParentNode!.PositionXChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.PositionYChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.WidthChanged -= self._OnNodePositionChanged;
            oldPin.ParentNode!.HeightChanged -= self._OnNodePositionChanged;
        }

        if (args.NewValue is NodePin newPin)
        {
            newPin.IsConnected = true;
            newPin.ParentNode!.PositionXChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.PositionYChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.WidthChanged += self._OnNodePositionChanged;
            newPin.ParentNode!.HeightChanged += self._OnNodePositionChanged;
        }
        else if (self.SourcePin is null)
        {
            throw new InvalidOperationException("SourcePin is null. At least one pin has to be connected.");
        }
        else
        {
            self._line.EndAngle = null;
        }

        self._UpdateLine();
        self._UpdateConnectionZIndex();
    }

    private double _GetPinConnectionAngle(NodePin pin)
    {
        // Check where the pin is located in the node. It can be top, bottom, left or right.
        // Depending on that, we have to set the angle of the connection.
        // The angle is the angle of the line that goes from the center of the node to the center of the pin.

        // Get the center of the node
        Point nodeCenter = CanvasExtension.GetCanvasControlCenter(pin.ParentNode!);
        // Get the center of the pin
        Point pinCenter = pin.GeCenterPositionRelativeToNode();
        double radiansAngle;
        switch(KB.SharpCore.Utils.Math.Get2DSideFromCenterToPoint(nodeCenter.X, nodeCenter.Y, pinCenter.X, pinCenter.Y))
        {
            case 0:
                //Top
                radiansAngle = KB.SharpCore.Utils.Math.PIHalf;
                break;
            case 1:
                //Left
                radiansAngle = System.Math.PI;
                break;
            case 2:
                //Bottom
                radiansAngle = (System.Math.PI * 3) / 2;
                break;
            case 3:
                //Right
                radiansAngle = 0;
                break;
            default:
                throw new InvalidOperationException("Pin is not located on any side of the node.");
        }

        return KB.SharpCore.Utils.Math.RadToDeg(radiansAngle);
    }

    private void _UpdateLine()
    {
        if(SourcePin is not null)
        {
            _line.StartPoint = SourcePin.GeCenterPositionRelativeToNode();
            _line.StartAngle = _GetPinConnectionAngle(SourcePin);
        }

        if(TargetPin is not null)
        {
            _line.EndPoint = TargetPin.GeCenterPositionRelativeToNode();
            _line.EndAngle = _GetPinConnectionAngle(TargetPin);
        }
    }

    private void _OnNodePositionChanged(object? sender, ValueChangedEventArgs<double> e)
    {
        _UpdateLine();
    }

    private void _UpdateConnectionZIndex()
    {
        // Connections have to be behind the nodes
        int? sourceNodeZIndez = SourcePin?.ParentNode?.ZIndex;
        int? targetNodeZIndez = TargetPin?.ParentNode?.ZIndex;

        if(sourceNodeZIndez.HasValue && targetNodeZIndez.HasValue)
        {
            this.ZIndex = sourceNodeZIndez <= targetNodeZIndez ? sourceNodeZIndez.Value - 1 : targetNodeZIndez.Value - 1;
        }
        else if(sourceNodeZIndez.HasValue)
        {
            this.ZIndex = sourceNodeZIndez.Value - 1;
        }
        else if(targetNodeZIndez.HasValue)
        {
            this.ZIndex = targetNodeZIndez.Value - 1;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PointerPressed?.Invoke(this, new NodeConnectionPointerInteractionEventArgs(this, e.GetPosition(this)));
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        PointerMoved?.Invoke(this, new NodeConnectionPointerInteractionEventArgs(this, e.GetPosition(this)));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        PointerReleased?.Invoke(this, new NodeConnectionPointerInteractionEventArgs(this, e.GetPosition(this)));
    }
}
