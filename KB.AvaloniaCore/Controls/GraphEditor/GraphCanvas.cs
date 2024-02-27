using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using KB.AvaloniaCore.Controls.GraphEditor.Events;
using KB.AvaloniaCore.Injection;
using KB.SharpCore.Utils;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace KB.AvaloniaCore.Controls.GraphEditor;

/// <summary>
/// Canvas to draw nodes and connections between them.
/// </summary>
public class GraphCanvas : Control
{
    /// <summary>
    /// Canvas to edit editable controls. Nodes.
    /// Drag and scale
    /// </summary>
    private readonly EditorCanvas _editorCanvas;

    /// <summary>
    /// Canvas to draw connections between nodes.
    /// </summary>
    private readonly Canvas _nodeConnectionsCanvas;

    private NodeConnection? _edittingConnection;

    static GraphCanvas()
    {
        GraphCanvas.ChildNodesProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s._OnChildNodesPropertyChanged(e));
        GraphCanvas.BackgroundProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s._OnBackgroundPropertyChanged(e));
    }

    public GraphCanvas()
    {
        _edittingConnection = null;

        _editorCanvas = new EditorCanvas();
        _nodeConnectionsCanvas = new Canvas();
        
        Grid canvasContainer = new Grid();
        canvasContainer.Children.Add(_editorCanvas);
        canvasContainer.Children.Add(_nodeConnectionsCanvas);

        // Bind the node canvas to the editor canvas so they have the same size
        Binding editorCanvasWidthBinding = new Binding(nameof(EditorCanvas.Width)) { Source = _editorCanvas, Mode = BindingMode.OneWay };
        Binding editorCanvasHeightBinding = new Binding(nameof(EditorCanvas.Height)) { Source = _editorCanvas, Mode = BindingMode.OneWay };
        _nodeConnectionsCanvas[!Canvas.WidthProperty] = editorCanvasWidthBinding;
        _nodeConnectionsCanvas[!Canvas.HeightProperty] = editorCanvasHeightBinding;
        LogicalChildren.Add(canvasContainer);
        VisualChildren.Add(canvasContainer);

        // Bind the selected items to the editor canvas
        Binding selectedItemsBinding = new Binding(nameof(EditorCanvas.SelectedItems)) { Source = _editorCanvas, Mode = BindingMode.TwoWay };
        this[!GraphCanvas.SelectedItemsProperty] = selectedItemsBinding;

        this.Background = Brushes.Purple;
        ChildNodes.CollectionChanged += _OnChildNodesCollectionChanged;
    }

    #region StyledProperties

    public readonly static StyledProperty<IAvaloniaList<Node>> ChildNodesProperty = AvaloniaProperty.Register<GraphCanvas, IAvaloniaList<Node>>(nameof(GraphCanvas.ChildNodes), new AvaloniaList<Node>());
    public readonly static StyledProperty<IEnumerable<NodeConnection>> NodeConnectionsProperty = AvaloniaProperty.Register<GraphCanvas, IEnumerable<NodeConnection>>(nameof(GraphCanvas.NodeConnections));
    public readonly static StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<GraphCanvas, IBrush>(nameof(GraphCanvas.Background), Brushes.White);

    public static readonly StyledProperty<AvaloniaList<IEditableControl>> SelectedItemsProperty
        = AvaloniaProperty.Register<GraphCanvas, AvaloniaList<IEditableControl>>(nameof(GraphCanvas.SelectedItems),
                                        defaultValue: new AvaloniaList<IEditableControl>(), defaultBindingMode: BindingMode.TwoWay);

    [Content]
    public IAvaloniaList<Node> ChildNodes
    {
        get { return GetValue(GraphCanvas.ChildNodesProperty); }
        set { SetValue(GraphCanvas.ChildNodesProperty, value); }
    }

    public IEnumerable<NodeConnection> NodeConnections
    {
        get { return GetValue(GraphCanvas.NodeConnectionsProperty); }
        set { SetValue(GraphCanvas.NodeConnectionsProperty, value); }
    }

    public IBrush Background
    {
        get { return GetValue(GraphCanvas.BackgroundProperty); }
        set { SetValue(GraphCanvas.BackgroundProperty, value); }
    }

    public AvaloniaList<IEditableControl> SelectedItems
    {
        get { return GetValue(GraphCanvas.SelectedItemsProperty); }
        set { SetValue(GraphCanvas.SelectedItemsProperty, value); }
    }

    #endregion

    private void _OnChildNodesPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(e.OldValue is INotifyCollectionChanged oldNotifyCollection)
        {
            oldNotifyCollection.CollectionChanged -= _OnChildNodesCollectionChanged;
        }

        if(e.NewValue != null)
        {
            if(e.NewValue is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += _OnChildNodesCollectionChanged;
            }
        }

        RebuildView();
    }

    private void _OnBackgroundPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _editorCanvas.Background = Background;
    }

    #region Node Management

    private void _OnChildNodesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Node node in e.NewItems)
            {
                _AddNode(node);
            }
        }

        if (e.OldItems != null)
        {
            foreach (Node node in e.OldItems)
            {
                _RemoveNode(node);
            }
        }

        InvalidateMeasure();
    }

    private void _AddNode(Node node)
    {
        _editorCanvas.Children.Add(node);
        node.ConnectionPinPressed += _OnNodePinPressed;
        node.ConnectionPinReleased += _OnNodePinReleased;
        node.ConnectionPinPointerMoved += _OnNodePinPointerMoved;
    }

    private bool _RemoveNode(Node node)
    {
        node.ConnectionPinPressed -= _OnNodePinPressed;
        node.ConnectionPinReleased -= _OnNodePinReleased;
        node.ConnectionPinPointerMoved -= _OnNodePinPointerMoved;
        return _editorCanvas!.Children.Remove(node);
    }

    #endregion

    #region NodeConnection Management

    private void _OnNodePinPressed(object? sender, NodePinPointerInteractionEventArgs args)
    {
        if(_edittingConnection != null)
        {
            // Something went wrong. We are already editting a connection
            // Remove the connection
            _RemoveEdittingConnection();
        }

        //Point point = args.Pin.ParentNode!.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        Point point = args.Pin.GeCenterPositionRelativeToNode();
        // Creating a new connection
        _edittingConnection = new NodeConnection(args.Pin, point, point);
        _nodeConnectionsCanvas.Children.Add(_edittingConnection);
        _edittingConnection.PointerPressed += _OnNodeConnectionPointerPressed;
        _edittingConnection.PointerMoved += _OnNodeConnectionPointerMoved;
        _edittingConnection.PointerReleased += _OnNodeConnectionPointerReleased;
    }

    private void _OnNodePinPointerMoved(object? sender, NodePinPointerInteractionEventArgs args)
    {
        if (_edittingConnection is null)
        {
            return;
        }

        Point point = args.Pin.GetPositionRelativeToParentNode(args.Point);
        _NodeConnectionDrag(point);
    }

    /// <summary>
    /// Fired when the pointer is released after a <see cref="NodePin"/> was clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void _OnNodePinReleased(object? sender, NodePinPointerInteractionEventArgs args)
    {
        if (_edittingConnection is null)
        {
            return;
        }

        Point point = args.Pin.GetPositionRelativeToParentNode(args.Point);
        _NodeConnectionEndInteraction(point);
    }

    private void _OnNodeConnectionPointerPressed(object? sender, NodeConnectionPointerInteractionEventArgs args)
    {
        _edittingConnection = _nodeConnectionsCanvas.Children.OfType<NodeConnection>().FirstOrDefault(x => x == args.Connection);
        if (_edittingConnection is null)
        {
            throw new Exception("Something went wrong. Clicking on a connection that does not belong to the graph.");
        }

        Point sourcePinCenter = args.Connection.SourcePin!.GeCenterPositionRelativeToNode();
        Point targetPinCenter = args.Connection.TargetPin!.GeCenterPositionRelativeToNode();
        double distanceToSource = KB.SharpCore.Utils.Math.GetDistanceBetweenPoints(args.Point.X, args.Point.Y, sourcePinCenter.X, sourcePinCenter.Y);
        double distanceToTarget = KB.SharpCore.Utils.Math.GetDistanceBetweenPoints(args.Point.X, args.Point.Y, targetPinCenter.X, targetPinCenter.Y);

        // We are disconnecting the pin
        NodePin closestPin = distanceToSource < distanceToTarget ? args.Connection.SourcePin! : args.Connection.TargetPin!;
        Point point = _edittingConnection.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        _edittingConnection.DisconnectPin(closestPin, point);
    }

    private void _OnNodeConnectionPointerMoved(object? sender, NodeConnectionPointerInteractionEventArgs args)
    {
        if (_edittingConnection is null)
        {
            return;
        }

        Point point = _edittingConnection.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        _NodeConnectionDrag(point);
    }

    private void _OnNodeConnectionPointerReleased(object? sender, NodeConnectionPointerInteractionEventArgs args)
    {
        Point releasePositionRelativeToCanvas = _edittingConnection!.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        _NodeConnectionEndInteraction(releasePositionRelativeToCanvas);
    }

    private void _NodeConnectionDrag(Point pointerPosition)
    {
        bool isSourcePin = _edittingConnection!.SourcePin is not null;
        // Update the opposite connection point
        if (isSourcePin)
        {
            _edittingConnection.UpdateEndPoint(pointerPosition);
        }
        else
        {
            _edittingConnection.UpdateStartPoint(pointerPosition);
        }
    }

    /// <summary>
    /// Check if the connection can be made and if so, connect the pins.
    /// </summary>
    /// <param name="pointerPosition"></param>
    private void _NodeConnectionEndInteraction(Point pointerPosition)
    {
        if(_edittingConnection == null)
        {
            return;
        }

        NodePin connectedPin = _edittingConnection.SourcePin is not null ? _edittingConnection.SourcePin : _edittingConnection.TargetPin!;

        NodePin? newPin = null;
        // Get the pin if we are over it
        foreach (IEditableControl nodeControl in _editorCanvas.Children)
        {
            Node targetNode = (Node)nodeControl.Control;
            foreach (NodePin targetNodePin in targetNode.GetAllConnectionPins())
            {
                Point targetPinPositionRelativeToNode = CanvasExtension.GetControlLeftTop(targetNodePin);
                Point targetPinRelativeToCanvasPosition = targetNodePin.ParentNode!.TranslatePoint(targetPinPositionRelativeToNode, _nodeConnectionsCanvas)!.Value;
                Point distanceBetweenNodePinAndReleasePoint = pointerPosition - targetPinRelativeToCanvasPosition;

                if (distanceBetweenNodePinAndReleasePoint.X.IsBetween(0, targetNodePin.Width) && distanceBetweenNodePinAndReleasePoint.Y.IsBetween(0, targetNodePin.Height))
                {
                    if (targetNode == connectedPin.ParentNode)
                    {
                        // Interacted pin belongs to the node we are over.
                        // Check that source and target pins do not have the same node. One connection can't be connected to the same node.
                        if (connectedPin.ParentNode == targetNode)
                        {
                            _RemoveEdittingConnection();
                            return;
                        }
                    }

                    // We are not over the pin
                    newPin = targetNodePin;
                    break;
                }
            }

            if (newPin is not null)
            {
                break;
            }
        }

        if (newPin is null)
        {
            // We are over the node but not over a pin
            _RemoveEdittingConnection();
            return;
        }

        if (!connectedPin.CanConnectToPin(newPin))
        {
            // The pin does not allow the connection
            _RemoveEdittingConnection();
            return;
        }

        // Passed all conditions. Connect pins
        _edittingConnection!.SetMissingPin(newPin);
        _edittingConnection = null;
    }

    private void _RemoveEdittingConnection()
    {
        if(_edittingConnection is null)
        {
            return;
        }

        _nodeConnectionsCanvas.Children.Remove(_edittingConnection);
        _edittingConnection.PointerPressed -= _OnNodeConnectionPointerPressed;
        _edittingConnection.PointerMoved -= _OnNodeConnectionPointerMoved;
        _edittingConnection.PointerReleased -= _OnNodeConnectionPointerReleased;
        _edittingConnection = null;
    }

    #endregion

    public void RebuildView()
    {
        _editorCanvas.Children.Clear();
        foreach (Node node in ChildNodes)
        {
            _AddNode(node);
        }
    }

    #region Inhertied Members

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        _RemoveEdittingConnection();
    }

    //public override void Render(DrawingContext context)
    //{
    //    base.Render(context);

    //    DrawGrid(context);
    //}

    //private void DrawGrid(DrawingContext context)
    //{
    //    Pen pen = new Pen(Brushes.LightGray, 1);
    //    double spacing = 25;

    //    for (double x = 0; x < Bounds.Width; x += spacing)
    //    {
    //        context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
    //    }

    //    for (double y = 0; y < Bounds.Height; y += spacing)
    //    {
    //        context.DrawLine(pen, new Point(0, y), new Point(Bounds.Width, y));
    //    }
    //}

    #endregion
}
