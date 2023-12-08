using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_ZoomDecorator", typeof(ZoomDecorator))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_Canvas", typeof(EditorCanvas))]
public class GraphCanvas : TemplatedControl
{
    private ZoomDecorator? _zoomDecorator;
    private ScrollViewer? _scrollViewer;
    private ContentPresenter? _contentPresenter;
    /// <summary>
    /// Canvas to edit editable controls. Nodes.
    /// Drag and scale
    /// </summary>
    private EditorCanvas? _canvas;

    static GraphCanvas()
    {
        ChildNodesProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s.m_OnChildNodesPropertyChanged(e));
    }

    public GraphCanvas()
    {
        
    }

    #region StyledProperties

    public readonly static StyledProperty<IAvaloniaList<Node>> ChildNodesProperty = AvaloniaProperty.Register<GraphCanvas, IAvaloniaList<Node>>(nameof(GraphCanvas.ChildNodes));
    public readonly static StyledProperty<IEnumerable<NodeConnection>> NodeConnectionsProperty = AvaloniaProperty.Register<GraphCanvas, IEnumerable<NodeConnection>>(nameof(GraphCanvas.NodeConnections));

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
        _canvas!.Children.Add(node);
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
        return _canvas!.Children.Remove(node);
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
        if(_canvas == null)
        {
            return;
        }

        _canvas.Children.Clear();
        foreach (Node node in ChildNodes)
        {
            _AddNode(node);
        }
    }

    #region Inhertied Members

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _zoomDecorator = e.NameScope.Get<ZoomDecorator>("PART_ZoomDecorator");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");
        _canvas = e.NameScope.Get<EditorCanvas>("PART_Canvas");
        RebuildView();
    }

    #endregion
}
