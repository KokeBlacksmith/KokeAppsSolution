using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_ZoomDecorator", typeof(ZoomDecorator))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class GraphCanvas : TemplatedControl
{
    private ZoomDecorator? _zoomDecorator;
    private ScrollViewer? _scrollViewer;
    private ContentPresenter? _contentPresenter;

    static GraphCanvas()
    {
        ChildNodesProperty.Changed.AddClassHandler<GraphCanvas>((s, e) => s.m_OnChildNodesPropertyChanged(e));
    }

    public GraphCanvas()
    {
        
    }

    #region StyledProperties

    public readonly static StyledProperty<IEnumerable<Node>> ChildNodesProperty = AvaloniaProperty.Register<GraphCanvas, IEnumerable<Node>>(nameof(GraphCanvas.ChildNodes));

    [Content]
    public IEnumerable<Node> ChildNodes
    {
        get { return GetValue(GraphCanvas.ChildNodesProperty); }
        set 
        { 
            SetValue(GraphCanvas.ChildNodesProperty, value);
        }
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
                _AddNode(node, true);
            }
        }

        if (e.OldItems != null)
        {
            foreach (var node in e.OldItems.OfType<Node>())
            {
                _RemoveNode(node, true);
            }
        }
    }

    private void _AddNode(Node node, bool addToCanvas)
    {
        if (addToCanvas)
        {
            
            //_zoomDecorator!.Children.Add(node);
        }

    }

    private void _DrawConnection(NodeConnection connection)
    {

    }

    private void _RemoveNode(Node node, bool removeFromCanvas)
    {
        if (removeFromCanvas)
        {
            //_zoomDecorator!.Children.Remove(node);
        }
    }

    #endregion

    #region NodeConnection Management



    #endregion

    public void RebuildView()
    {
        foreach (Node node in ChildNodes)
        {
            _AddNode(node, false);
        }
    }

    #region Inhertied Members

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _zoomDecorator = e.NameScope.Get<ZoomDecorator>("PART_ZoomDecorator");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");
    }

    #endregion
}
