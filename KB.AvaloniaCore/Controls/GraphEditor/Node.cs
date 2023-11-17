using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
//[TemplatePart("PART_Border", typeof(Border))]
public class Node : TemplatedControl
{
    //private Border? _border;
    private ContentPresenter? _contentPresenter;

    static Node()
    {
        AffectsMeasure<Node>(ChildProperty, PaddingProperty);
        Node.ChildProperty.Changed.AddClassHandler<Node>((s, e) => s.m_OnChildPropertyChanged(e));
    }

    #region StyledProperties

    public static readonly StyledProperty<Control?> ChildProperty = AvaloniaProperty.Register<Node, Control?>(nameof(Node.Child));
    public static readonly StyledProperty<double> PositionXProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionX));
    public static readonly StyledProperty<double> PositionYProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionY));
    
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsSelected));
    public static readonly StyledProperty<bool> IsDraggedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsDragged));

    [Content]
    public Control? Child
    {
        get { return GetValue(Node.ChildProperty); }
        set { SetValue(Node.ChildProperty, value); }
    }

    public double PositionX
    {
        get { return GetValue(Node.PositionXProperty); }
        set { SetValue(Node.PositionXProperty, value); }
    }

    public double PositionY
    {
        get { return GetValue(Node.PositionYProperty); }
        set { SetValue(Node.PositionYProperty, value); }
    }

    public bool IsSelected
    {
        get { return GetValue(Node.IsSelectedProperty); }
        set { SetValue(Node.IsSelectedProperty, value); }
    }

    public bool IsDragged
    {
        get { return GetValue(Node.IsDraggedProperty); }
        set { SetValue(Node.IsDraggedProperty, value); }
    }

    #endregion

    #region Inhertied Members

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(Child, availableSize, Padding);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        return LayoutHelper.ArrangeChild(Child, finalSize, Padding);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        //_border = e.NameScope.Get<Border>("PART_Border");
        //_border.Child = Child;
        _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");
        _contentPresenter.Content = new Border() 
        { 
            Background = Brushes.Yellow,
            CornerRadius = new CornerRadius(20d)
        };
    }

    #endregion

    private void m_OnChildPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        //if(_contentPresenter == null)
        //{
        //    return;
        //}

        var oldChild = (Control?)e.OldValue;
        var newChild = (Control?)e.NewValue;

        //_border!.Child = newChild;

        if (oldChild != null)
        {
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
        }
    }
}
