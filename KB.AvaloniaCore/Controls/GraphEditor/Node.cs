using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public abstract class Node : Control
{
    /// <summary>
    /// Container of all the elements of the node.
    /// </summary>
    private readonly Panel? _panel;

    /// <summary>
    /// Container to add node connections to.
    /// </summary>
    /// 
    private readonly Canvas? _canvas;

    /// <summary>
    /// Container of the <see cref="Child"/> control
    /// </summary>
    private readonly Border? _border;

    static Node()
    {
        Node.ChildProperty.Changed.AddClassHandler<Node>((s, e) => s.m_OnChildPropertyChanged(e));
        Node.BackgroundProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBackgroundPropertyChanged(e));
        Node.BorderBrushProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderBrushPropertyChanged(e));
        Node.BorderThicknessProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderThicknessPropertyChanged(e));
        Node.CornerRadiusProperty.Changed.AddClassHandler<Node>((s, e) => s._OnCornerRadiusPropertyChanged(e));
        Node.PaddingProperty.Changed.AddClassHandler<Node>((s, e) => s._OnPaddingPropertyChanged(e));
    }

    public Node()
    {
        _canvas = new Canvas()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        _border = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };

        _panel = new Panel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        _panel.Children.Add(_canvas);
        _panel.Children.Add(_border);

        ((ISetLogicalParent)_panel).SetParent(this);
        VisualChildren.Add(_panel);
        LogicalChildren.Add(_panel);
    }

    #region StyledProperties

    public static readonly StyledProperty<Control?> ChildProperty = AvaloniaProperty.Register<Node, Control?>(nameof(Node.Child));
    public static readonly StyledProperty<double> PositionXProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionX));
    public static readonly StyledProperty<double> PositionYProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionY));
    
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsSelected));
    public static readonly StyledProperty<bool> IsDraggedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsDragged));

    public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(Node.Background), Brushes.White);
    public static readonly StyledProperty<IBrush> BorderBrushProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(Node.BorderBrush), Brushes.Black);
    public static readonly StyledProperty<Thickness> BorderThicknessProperty = AvaloniaProperty.Register<Node, Thickness>(nameof(Node.BorderThickness), new Thickness(1));
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<Node, CornerRadius>(nameof(Node.CornerRadius), new CornerRadius(0));
    public static readonly StyledProperty<Thickness> PaddingProperty = AvaloniaProperty.Register<Node, Thickness>(nameof(Node.Padding), new Thickness(0));


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

    public IBrush Background
    {
        get { return GetValue(Node.BackgroundProperty); }
        set { SetValue(Node.BackgroundProperty, value); }
    }

    public IBrush BorderBrush
    {
        get { return GetValue(Node.BorderBrushProperty); }
        set { SetValue(Node.BorderBrushProperty, value); }
    }

    public Thickness BorderThickness
    {
        get { return GetValue(Node.BorderThicknessProperty); }
        set { SetValue(Node.BorderThicknessProperty, value); }
    }

    public CornerRadius CornerRadius
    {
        get { return GetValue(Node.CornerRadiusProperty); }
        set { SetValue(Node.CornerRadiusProperty, value); }
    }

    public Thickness Padding
    {
        get { return GetValue(Node.PaddingProperty); }
        set { SetValue(Node.PaddingProperty, value); }
    }

    #endregion

    #region Inhertied Members

  

    #endregion

    private void m_OnChildPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.Child = (Control?)e.NewValue; ;
    }

    private void _OnBackgroundPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.Background = e.NewValue as IBrush;
    }

    private void _OnBorderBrushPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.BorderBrush = e.NewValue as IBrush;
    }

    private void _OnBorderThicknessPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.BorderThickness = (Thickness)e.NewValue!;
    }


    private void _OnCornerRadiusPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.CornerRadius = (CornerRadius)e.NewValue!;
    }

    private void _OnPaddingPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _border!.Padding = (Thickness)e.NewValue!;
    }
}
