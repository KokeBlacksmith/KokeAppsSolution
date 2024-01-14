using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using KB.AvaloniaCore.Injection;
using Microsoft.CodeAnalysis.Text;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_OutterBorder", typeof(Border))]
[TemplatePart("PART_InnerBorder", typeof(Border))]
[PseudoClasses(":connected")]
public class NodePin : TemplatedControl
{
    private const double c_InteractionHelperSize = 10d;

    private Border? _outterBorder;
    private Border? _innerBorder;

    /// <summary>
    /// Control that is used to handle mouse events. It is biger than the pin itself, so it is easier to click on it.
    /// </summary>
    private readonly Border _interactionHelper;

    static NodePin()
    {
        IsConnectedProperty.Changed.AddClassHandler<NodePin>((s, e) => s.m_OnIsConnectedPropertyChanged(e));
        OutterBrushProperty.Changed.AddClassHandler<NodePin>((s, e) => s.m_OnOutterBrushPropertyChanged(e));
        InnerBrushProperty.Changed.AddClassHandler<NodePin>((s, e) => s.m_OnInnerBrushPropertyChanged(e));
        ThicknessProperty.Changed.AddClassHandler<NodePin>((s, e) => s.m_OnThicknessPropertyChanged(e));
    }

    public NodePin()
    {
        ClipToBounds = false;
        _interactionHelper = new Border();
    }

    public Node? ParentNode { get; private set; }

    #region StyledProperties

    public static readonly StyledProperty<bool> IsConnectedProperty = AvaloniaProperty.Register<NodePin, bool>(nameof(NodePin.IsConnected));
    public static readonly StyledProperty<IBrush> OutterBrushProperty = AvaloniaProperty.Register<NodePin, IBrush>(nameof(NodePin.OutterBrush), Brushes.Black);
    public static readonly StyledProperty<IBrush> InnerBrushProperty = AvaloniaProperty.Register<NodePin, IBrush>(nameof(NodePin.InnerBrush), Brushes.White);
    public static readonly StyledProperty<Thickness> ThicknessProperty = AvaloniaProperty.Register<NodePin, Thickness>(nameof(NodePin.Thickness), new Thickness(1));

    #endregion

    public bool IsConnected
    {
        get { return GetValue(NodePin.IsConnectedProperty); }
        set { SetValue(NodePin.IsConnectedProperty, value); }
    }

    public IBrush OutterBrush
    {
        get { return GetValue(NodePin.OutterBrushProperty); }
        set { SetValue(NodePin.OutterBrushProperty, value); }
    }

    public IBrush InnerBrush
    {
        get { return GetValue(NodePin.InnerBrushProperty); }
        set { SetValue(NodePin.InnerBrushProperty, value); }
    }

    public Thickness Thickness
    {
        get { return GetValue(NodePin.ThicknessProperty); }
        set { SetValue(NodePin.ThicknessProperty, value); }
    }

    /// <summary>
    /// Returns true if the given connection pin can be connected to this pin.
    /// <para/>
    /// Some nodes may want to override it to allow only specific pins to be connected.
    /// </summary>
    /// <param name="targetPin"></param>
    /// <returns></returns>
    public virtual bool CanConnectToPin(NodePin targetPin)
    {
        return true;
    }

    public Point GeCenterPositionRelativeToNode()
    {
        Point pinPosition = CanvasExtension.GetCanvasControlCenter(this);
        return this.GetPositionRelativeToParentNode(pinPosition);
    }

    public Point GetPositionRelativeToParentNode(Point pinRelativePosition)
    {
        Canvas parentNodeCanvas = this.ParentNode!.GetParentOfType<Canvas>();
        return this.ParentNode!.TranslatePoint(pinRelativePosition, parentNodeCanvas)!.Value;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _outterBorder = e.NameScope.Find<Border>("PART_OutterBorder");
        _outterBorder!.BorderBrush = OutterBrush;
        _innerBorder = e.NameScope.Find<Border>("PART_InnerBorder");
        _innerBorder!.Background = InnerBrush;

        _interactionHelper.Background = Brushes.Transparent;
        LogicalChildren.Add(_interactionHelper);
        VisualChildren.Add(_interactionHelper);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ParentNode = this.GetParentOfType<Node>();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ParentNode = null;
    }

    private void m_OnIsConnectedPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        //if ((bool)e.NewValue!)
        //{
        //    this.Classes.Add(":connected");
        //}
        //else
        //{
        //    this.Classes.Remove(":connected");
        //}
    }

    private void m_OnInnerBrushPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(_outterBorder == null)
        {
            return;
        }

        _outterBorder!.BorderBrush = e.NewValue as IBrush;
    }

    private void m_OnOutterBrushPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(_innerBorder == null)
        {
            return;
        }

        _innerBorder!.Background = e.NewValue as IBrush;
    }

    private void m_OnThicknessPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _outterBorder!.BorderThickness = (Thickness)e.NewValue!;
    }

    /// <summary>
    /// Calculate the desired size of the control, including the helper decorator
    /// Helper decorator is <see cref="c_InteractionHelperSize"/> points bigger on each side than the control itself
    /// </summary>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Measure the desired size of the control itself
        Size size = base.MeasureOverride(availableSize);

        // Adjust the size for the decorator, adding 5 points on each side
        Size decoratorSize = new Size(size.Width + c_InteractionHelperSize, size.Height + c_InteractionHelperSize);
        _interactionHelper.Measure(decoratorSize);

        return size;
    }

    /// <summary>
    /// Arrange the control, including the helper decorator
    /// Helper decorator is <see cref="c_InteractionHelperSize"/> points bigger on each side than the control itself
    /// </summary>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Arrange the control itself
        Size size = base.ArrangeOverride(finalSize);

        // Arrange the decorator, offsetting its position and adding 5 points on each side
        Rect decoratorRect = new Rect(-c_InteractionHelperSize * 0.5d, -c_InteractionHelperSize * 0.5d, size.Width + c_InteractionHelperSize, size.Height + c_InteractionHelperSize);
        _interactionHelper.Arrange(decoratorRect);

        return size;
    }
}
