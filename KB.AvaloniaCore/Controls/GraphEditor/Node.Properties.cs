using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia;
using KB.SharpCore.Events;
using KB.AvaloniaCore.Controls.GraphEditor.Events;
using System.ComponentModel;

namespace KB.AvaloniaCore.Controls.GraphEditor;

/// <summary>
/// Styled properties and default propeties and accessors of a node in a graph.
/// It does not contain logic. All the logic is in <see cref="Node"/>.
/// </summary>
public abstract partial class Node : IEditableControl
{

    #region Fields

    protected Guid m_id = new Guid();

    protected List<NodePin> m_leftConnectionPins = new List<NodePin>();
    protected List<NodePin> m_rightConnectionPins = new List<NodePin>();
    protected List<NodePin> m_topConnectionPins = new List<NodePin>();
    protected List<NodePin> m_bottomConnectionPins = new List<NodePin>();

    #endregion

    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<double>>? PositionXChanged;
    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<double>>? PositionYChanged;
    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<double>>? WidthChanged;
    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<double>>? HeightChanged;
    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<bool>>? IsSelectedChanged;

    public event EventHandler<NodePinPointerInteractionEventArgs>? ConnectionPinPressed;
    public event EventHandler<NodePinPointerInteractionEventArgs>? ConnectionPinReleased;
    public event EventHandler<NodePinPointerInteractionEventArgs>? ConnectionPinPointerMoved;

    static Node()
    {
        Node.ChildProperty.Changed.AddClassHandler<Node>((s, e) => s.m_OnChildPropertyChanged(e));
        Node.BackgroundProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBackgroundPropertyChanged(e));
        Node.BorderBrushProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderBrushPropertyChanged(e));
        Node.BorderThicknessProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderThicknessPropertyChanged(e));
        Node.CornerRadiusProperty.Changed.AddClassHandler<Node>((s, e) => s._OnCornerRadiusPropertyChanged(e));
        Node.PaddingProperty.Changed.AddClassHandler<Node>((s, e) => s._OnPaddingPropertyChanged(e));

        Node.PositionXProperty.Changed.AddClassHandler<Node>((s, e) => s._OnPositionXPropertyChanged(e));
        Node.PositionYProperty.Changed.AddClassHandler<Node>((s, e) => s._OnPositionYPropertyChanged(e));
        Node.WidthProperty.Changed.AddClassHandler<Node>((s, e) => s._OnWidthPropertyChanged(e));
        Node.HeightProperty.Changed.AddClassHandler<Node>((s, e) => s._OnHeightPropertyChanged(e));
        Node.IsSelectedProperty.Changed.AddClassHandler<Node>((s, e) => s._OnIsSelectedPropertyChanged(e));
    }

    #region StyledProperties

    public static readonly StyledProperty<Control?> ChildProperty = AvaloniaProperty.Register<Node, Control?>(nameof(Node.Child));
    public static readonly StyledProperty<double> PositionXProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionX));
    public static readonly StyledProperty<double> PositionYProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionY));

    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsSelected));

    public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(Node.Background), Brushes.White);
    public static readonly StyledProperty<IBrush> BorderBrushProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(Node.BorderBrush), Brushes.Black);
    public static readonly StyledProperty<Thickness> BorderThicknessProperty = AvaloniaProperty.Register<Node, Thickness>(nameof(Node.BorderThickness), new Thickness(1));
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<Node, CornerRadius>(nameof(Node.CornerRadius), new CornerRadius(0));
    public static readonly StyledProperty<Thickness> PaddingProperty = AvaloniaProperty.Register<Node, Thickness>(nameof(Node.Padding), new Thickness(0));

    [Browsable(false)]
    [Content]
    public Control? Child
    {
        get { return GetValue(Node.ChildProperty); }
        set { SetValue(Node.ChildProperty, value); }
    }

    [Category("Transform")]
    [Description("The X position of the node.")]
    [Bindable(true)]
    [Browsable(true)]
    public double PositionX
    {
        get { return GetValue(Node.PositionXProperty); }
        set { SetValue(Node.PositionXProperty, value); }
    }

    [Category("Transform")]
    [Description("The Y position of the node.")]
    [Bindable(true)]
    public double PositionY
    {
        get { return GetValue(Node.PositionYProperty); }
        set { SetValue(Node.PositionYProperty, value); }
    }

    [Browsable(false)]
    public bool IsSelected
    {
        get { return GetValue(Node.IsSelectedProperty); }
        set { SetValue(Node.IsSelectedProperty, value); }
    }

    [Browsable(false)]
    public IBrush Background
    {
        get { return GetValue(Node.BackgroundProperty); }
        set { SetValue(Node.BackgroundProperty, value); }
    }

    [Browsable(false)]
    public IBrush BorderBrush
    {
        get { return GetValue(Node.BorderBrushProperty); }
        set { SetValue(Node.BorderBrushProperty, value); }
    }

    [Browsable(false)]
    public Thickness BorderThickness
    {
        get { return GetValue(Node.BorderThicknessProperty); }
        set { SetValue(Node.BorderThicknessProperty, value); }
    }

    [Browsable(false)]
    public CornerRadius CornerRadius
    {
        get { return GetValue(Node.CornerRadiusProperty); }
        set { SetValue(Node.CornerRadiusProperty, value); }
    }

    [Browsable(false)]
    public Thickness Padding
    {
        get { return GetValue(Node.PaddingProperty); }
        set { SetValue(Node.PaddingProperty, value); }
    }

    public Control Control => this;

    #endregion

    #region Properties

    public Guid Id => m_id;
    public IReadOnlyList<NodePin> LeftConnectionPins => m_leftConnectionPins;
    public IReadOnlyList<NodePin> RightConnectionPins => m_rightConnectionPins;
    public IReadOnlyList<NodePin> TopConnectionPins => m_topConnectionPins;
    public IReadOnlyList<NodePin> BottomConnectionPins => m_bottomConnectionPins;

    #endregion

    /// <summary>
    /// Returns all the connection pins of this node.
    /// The order is left, right, top, bottom.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NodePin> GetAllConnectionPins()
    {
        foreach(NodePin pin in m_leftConnectionPins)
        {
            yield return pin;
        }

        foreach (NodePin pin in m_rightConnectionPins)
        {
            yield return pin;
        }

        foreach (NodePin pin in m_topConnectionPins)
        {
            yield return pin;
        }

        foreach (NodePin pin in m_bottomConnectionPins)
        {
            yield return pin;
        }
    }
}
