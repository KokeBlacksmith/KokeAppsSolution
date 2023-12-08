using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public class GraphCanvas : Control
{
    private readonly ZoomDecorator _zoomDecorator;
    private readonly ScrollViewer _scrollViewer;
    /// <summary>
    /// Canvas to edit editable controls. Nodes.
    /// Drag and scale
    /// </summary>
    private readonly EditorCanvas _editorCanvas;

    static GraphCanvas()
    {
        ChildNodesProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s.m_OnChildNodesPropertyChanged(e));
        BackgroundProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s._OnBackgroundPropertyChanged(e));
    }

    public GraphCanvas()
    {
        _scrollViewer = new ScrollViewer();
        _zoomDecorator = new ZoomDecorator();
        _editorCanvas = new EditorCanvas();

        _scrollViewer.Content = _zoomDecorator;
        _zoomDecorator.Child = _editorCanvas;

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
        if (e.NewItems != null)
        {
            foreach (var node in e.NewItems.OfType<Node>())
            {
                _AddNode(node);
            }
        }

        if (e.OldItems != null)
        {
            foreach (var node in e.OldItems.OfType<Node>())
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
        //_UpdateNodePosition(node);
    }

    private void _DrawConnection(NodeConnection connection)
    {

    }

    private bool _RemoveNode(Node node)
    {
        node.ConnectionPinPressed -= _OnNodePinPressed;
        node.ConnectionPinReleased -= _OnNodePinReleased;
        return _editorCanvas!.Children.Remove(node);
    }

    #endregion

    #region NodeConnection Management
    private void _OnNodePinPressed(NodeConnectionPin pin)
    {

    }

    private void _OnNodePinReleased(NodeConnectionPin pin)
    {

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
    //protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    //{
    //    base.OnAttachedToVisualTree(e);
    //    RebuildView();
    //}

    #endregion
}
