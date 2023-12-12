using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using KB.AvaloniaCore.Controls.GraphEditor.Events;
using KB.AvaloniaCore.Injection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public class GraphCanvas : Control
{
    //TODO: Move this controls to EditorCanvas
    private readonly ZoomDecorator _zoomDecorator;
    private readonly ScrollViewer _scrollViewer;



    /// <summary>
    /// Canvas to edit editable controls. Nodes.
    /// Drag and scale
    /// </summary>
    private readonly EditorCanvas _editorCanvas;

    /// <summary>
    /// Canvas to draw connections between nodes.
    /// </summary>
    private readonly Canvas _nodeConnectionsCanvas;

    private readonly NodeConnectionCollection _nodeConnections;
    private NodeConnection? _edittingConnection;

    static GraphCanvas()
    {
        ChildNodesProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s.m_OnChildNodesPropertyChanged(e));
        BackgroundProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s._OnBackgroundPropertyChanged(e));
    }

    public GraphCanvas()
    {
        _edittingConnection = null;
        _nodeConnections = new NodeConnectionCollection();
        _scrollViewer = new ScrollViewer();
        _zoomDecorator = new ZoomDecorator();
        _editorCanvas = new EditorCanvas();
        _nodeConnectionsCanvas = new Canvas();

        _scrollViewer.Content = _zoomDecorator;
        Grid canvasContainer = new Grid();
        canvasContainer.Children.Add(_editorCanvas);
        canvasContainer.Children.Add(_nodeConnectionsCanvas);
        _zoomDecorator.Child = canvasContainer;

        LogicalChildren.Add(_scrollViewer);
        VisualChildren.Add(_scrollViewer);

        this.Background = Brushes.Purple;
    }

    #region StyledProperties

    public readonly static StyledProperty<IAvaloniaList<Node>> ChildNodesProperty = AvaloniaProperty.Register<GraphCanvas, IAvaloniaList<Node>>(nameof(GraphCanvas.ChildNodes));
    public readonly static StyledProperty<IEnumerable<NodeConnection>> NodeConnectionsProperty = AvaloniaProperty.Register<GraphCanvas, IEnumerable<NodeConnection>>(nameof(GraphCanvas.NodeConnections));
    public readonly static StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<GraphCanvas, IBrush>(nameof(GraphCanvas.Background), Brushes.White);

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

    #endregion

    protected virtual void m_OnChildNodesPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(e.OldValue is INotifyCollectionChanged oldNotifyCollection)
        {
            oldNotifyCollection.CollectionChanged -= m_OnChildNodesCollectionChanged;
        }

        if(e.NewValue != null)
        {
            if(e.NewValue is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += m_OnChildNodesCollectionChanged;
            }
        }

        RebuildView();
    }

    private void _OnBackgroundPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _editorCanvas.Background = Background;
    }


    #region Node Management

    private void m_OnChildNodesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is IEnumerable<Node> newNodes)
        {
            foreach (Node node in newNodes)
            {
                _AddNode(node);
            }
        }

        if (e.OldItems is IEnumerable<Node> oldNodes)
        {
            foreach (Node node in oldNodes)
            {
                _RemoveNode(node);
            }
        }
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
        Point point = args.Pin.ParentNode!.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        _edittingConnection = _nodeConnections.GetConnection(args.Pin);
        if(_edittingConnection is not null)
        {
            // We are disconnecting the pin
            // Remove the connection
            _nodeConnections.Remove(_edittingConnection);
            
        }
        else
        {
            // Creating a new connection
            _edittingConnection = new NodeConnection(args.Pin, point, point);
            _nodeConnectionsCanvas.Children.Add(_edittingConnection);
        }
    }

    private void _OnNodePinPointerMoved(object? sender, NodePinPointerInteractionEventArgs args)
    {
        if (_edittingConnection is null)
        {
            return;
        }

        Point point = args.Pin.ParentNode!.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;
        // Update the opposite connection point
        if (_edittingConnection.SourcePin == args.Pin)
        {
            _edittingConnection.UpdateEndPoint(point);
        }
        else if (_edittingConnection.TargetPin == args.Pin)
        {
            _edittingConnection.UpdateStartPoint(point);
        }
        else
        {
            throw new Exception("Updating a pin that is not being editted. An unhandled exception occurred.");
        }
    }

    private void _OnNodePinReleased(object? sender, NodePinPointerInteractionEventArgs args)
    {
        Node? targetNode = null;
        Point releasePoint = args.Pin.ParentNode!.TranslatePoint(args.Point, _nodeConnectionsCanvas)!.Value;

        // Get the pin if we are over it
        foreach (IEditableControl nodeControl in _editorCanvas.Children)
        {
            if(CanvasExtension.IsPointOverCanvasChild(releasePoint, nodeControl.Control))
            {
                targetNode = (Node)nodeControl.Control;
                if(targetNode == args.Pin.Parent)
                {
                    // Pin is over the same node we are connecting
                    _RemoveEdittingConnection();
                    return;
                }

                break;
            }
        }

        if(targetNode is null) 
        {
            // We are not over a node
            _RemoveEdittingConnection();
            return;
        }

        NodePin? newPin = null;
        foreach(NodePin targetNodePin in targetNode.GetAllConnectionPins())
        {
            //TODO: Get relative position of the nodePin
            //Get relative position of the nodePin
            // Check if we are over the pin by its bounds
            //if (CanvasExtension.IsPointOverCanvasChild(point, nodePin))
            //{
            //    newPin = nodePin;
            //    break;
            //}



            Point targetPinPosition = CanvasExtension.GetControlLeftTop(targetNodePin);
            Point targetPinRelativePosition = targetNodePin.ParentNode!.TranslatePoint(targetPinPosition, _nodeConnectionsCanvas)!.Value;
            Point tmpPoint = releasePoint - targetPinPosition;
            if(targetNodePin.Bounds.Contains(tmpPoint))
            {
                // We are not over the pin
                newPin = targetNodePin;
                break;
            }
        }

        if(newPin is null)
        {
            // We are over the node but not over a pin
            _RemoveEdittingConnection();
            return;
        }

        if(!args.Pin.CanConnectToPin(newPin))
        {
            // The pin does not allow the connection
            _RemoveEdittingConnection();
            return;
        }

        // Passed all conditions. Connect pins
        _edittingConnection!.SetMissingPin(newPin);
        _nodeConnections.Add(_edittingConnection);
    }

    private void _RemoveEdittingConnection()
    {
        if(_edittingConnection is null)
        {
            return;
        }

        _nodeConnectionsCanvas.Children.Remove(_edittingConnection);
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

    #endregion
}
