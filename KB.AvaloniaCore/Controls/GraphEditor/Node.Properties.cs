using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.GraphEditor;

/// <summary>
/// Styled properties of a node in a graph.
/// </summary>
public abstract partial class Node
{
    static Node()
    {
        Node.ChildProperty.Changed.AddClassHandler<Node>((s, e) => s.m_OnChildPropertyChanged(e));
        Node.BackgroundProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBackgroundPropertyChanged(e));
        Node.BorderBrushProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderBrushPropertyChanged(e));
        Node.BorderThicknessProperty.Changed.AddClassHandler<Node>((s, e) => s._OnBorderThicknessPropertyChanged(e));
        Node.CornerRadiusProperty.Changed.AddClassHandler<Node>((s, e) => s._OnCornerRadiusPropertyChanged(e));
        Node.PaddingProperty.Changed.AddClassHandler<Node>((s, e) => s._OnPaddingPropertyChanged(e));
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
}
