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
    }

    #region StyledProperties

    public static readonly StyledProperty<bool> IsConnectedProperty = AvaloniaProperty.Register<NodeConnectionPin, bool>(nameof(NodeConnectionPin.IsConnected));

    #endregion

    public bool IsConnected
    {
        get { return GetValue(NodeConnectionPin.IsConnectedProperty); }
        set { SetValue(NodeConnectionPin.IsConnectedProperty, value); }
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _outterBorder = e.NameScope.Find<Border>("PART_OutterBorder");
        _innerBorder = e.NameScope.Find<Border>("PART_InnerBorder");
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
}
