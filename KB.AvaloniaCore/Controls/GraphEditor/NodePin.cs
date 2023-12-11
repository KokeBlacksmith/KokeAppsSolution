using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using KB.AvaloniaCore.Injection;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_OutterBorder", typeof(Border))]
[TemplatePart("PART_InnerBorder", typeof(Border))]
public class NodePin : TemplatedControl
{
    private Border? _outterBorder;
    private Border? _innerBorder;
    

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _outterBorder = e.NameScope.Find<Border>("PART_OutterBorder");
        _outterBorder!.BorderBrush = OutterBrush;
        _innerBorder = e.NameScope.Find<Border>("PART_InnerBorder");
        _innerBorder!.Background = InnerBrush;
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
        //if (_border != null && _Border != null)
        //{
        //    if ((bool)e.NewValue!)
        //    {
        //        _border.BorderBrush = Brushes.Black;
        //        _Border.Fill = Brushes.Black;
        //    }
        //    else
        //    {
        //        _border.BorderBrush = Brushes.Transparent;
        //        _Border.Fill = Brushes.Transparent;
        //    }
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
}
