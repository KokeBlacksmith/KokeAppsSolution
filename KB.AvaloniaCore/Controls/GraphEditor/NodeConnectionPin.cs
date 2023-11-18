using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_OutterBorder", typeof(Border))]
[TemplatePart("PART_InnerBorder", typeof(Border))]
public class NodeConnectionPin : TemplatedControl
{
    private Border? _outterBorder;
    private Border? _innerBorder;

    static NodeConnectionPin()
    {
        IsConnectedProperty.Changed.AddClassHandler<NodeConnectionPin>((s, e) => s.m_OnIsConnectedPropertyChanged(e));
        OutterBrushProperty.Changed.AddClassHandler<NodeConnectionPin>((s, e) => s.m_OnOutterBrushPropertyChanged(e));
        InnerBrushProperty.Changed.AddClassHandler<NodeConnectionPin>((s, e) => s.m_OnInnerBrushPropertyChanged(e));
    }

    public NodeConnectionPin()
    {
        ClipToBounds = false;
    }

    #region StyledProperties

    public static readonly StyledProperty<bool> IsConnectedProperty = AvaloniaProperty.Register<NodeConnectionPin, bool>(nameof(NodeConnectionPin.IsConnected));
    public static readonly StyledProperty<IBrush> OutterBrushProperty = AvaloniaProperty.Register<NodeConnectionPin, IBrush>(nameof(NodeConnectionPin.OutterBrush), Brushes.Black);
    public static readonly StyledProperty<IBrush> InnerBrushProperty = AvaloniaProperty.Register<NodeConnectionPin, IBrush>(nameof(NodeConnectionPin.InnerBrush), Brushes.White);

    #endregion

    public bool IsConnected
    {
        get { return GetValue(NodeConnectionPin.IsConnectedProperty); }
        set { SetValue(NodeConnectionPin.IsConnectedProperty, value); }
    }

    public IBrush OutterBrush
    {
        get { return GetValue(NodeConnectionPin.OutterBrushProperty); }
        set { SetValue(NodeConnectionPin.OutterBrushProperty, value); }
    }

    public IBrush InnerBrush
    {
        get { return GetValue(NodeConnectionPin.InnerBrushProperty); }
        set { SetValue(NodeConnectionPin.InnerBrushProperty, value); }
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _outterBorder = e.NameScope.Find<Border>("PART_OutterBorder");
        _outterBorder!.BorderBrush = OutterBrush;
        _innerBorder = e.NameScope.Find<Border>("PART_InnerBorder");
        _innerBorder!.Background = InnerBrush;
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
}
