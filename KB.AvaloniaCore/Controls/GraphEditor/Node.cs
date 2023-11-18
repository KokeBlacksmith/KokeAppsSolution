using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public abstract partial class Node : Control
{
    /// <summary>
    /// Container of all the elements of the node.
    /// </summary>
    private readonly Panel? _mainPanel;

    /// <summary>
    /// Container to add node connections to.
    /// </summary>
    /// 
    private readonly Canvas? _connectionsCanvas;

    /// <summary>
    /// Container of the <see cref="Child"/> control
    /// </summary>
    private readonly Border? _childParentBorder;



    public Node()
    {
        _connectionsCanvas = new Canvas()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        _childParentBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };

        _mainPanel = new Panel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        _mainPanel.Children.Add(_connectionsCanvas);
        _mainPanel.Children.Add(_childParentBorder);

        ((ISetLogicalParent)_mainPanel).SetParent(this);
        VisualChildren.Add(_mainPanel);
        LogicalChildren.Add(_mainPanel);
    }


    #region Inhertied Members



    #endregion

    #region PropertyChangedEvents

    private void m_OnChildPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.Child = (Control?)e.NewValue; ;
    }

    private void _OnBackgroundPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.Background = e.NewValue as IBrush;
    }

    private void _OnBorderBrushPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.BorderBrush = e.NewValue as IBrush;
    }

    private void _OnBorderThicknessPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.BorderThickness = (Thickness)e.NewValue!;
    }


    private void _OnCornerRadiusPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.CornerRadius = (CornerRadius)e.NewValue!;
    }

    private void _OnPaddingPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _childParentBorder!.Padding = (Thickness)e.NewValue!;
    }

    #endregion
}
