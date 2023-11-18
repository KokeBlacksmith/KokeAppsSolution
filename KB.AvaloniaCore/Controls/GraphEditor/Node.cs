using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using KB.SharpCore.Enums;

namespace KB.AvaloniaCore.Controls.GraphEditor;

/// <summary>
/// Logic of a node in a graph.
/// </summary>
public abstract partial class Node : Control
{
    #region Fields

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

    #endregion


    public Node()
    {
        _childParentBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };

        _connectionsCanvas = new Canvas()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        _mainPanel = new Panel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        // Order is important, the last added element is the topmost.
        _mainPanel.Children.Add(_childParentBorder);
        _mainPanel.Children.Add(_connectionsCanvas);

        ((ISetLogicalParent)_mainPanel).SetParent(this);
        VisualChildren.Add(_mainPanel);
        LogicalChildren.Add(_mainPanel);
    }


    #region Methods

    public void AddConnectionPin(NodeConnectionPin pin, ESide side)
    {
        switch (side)
        {
            case ESide.Left:
                m_leftConnectionPins.Add(pin);
                break;
            case ESide.Right:
                m_rightConnectionPins.Add(pin);
                break;
            case ESide.Top:
                m_topConnectionPins.Add(pin);
                break;
            case ESide.Bottom:
                m_bottomConnectionPins.Add(pin);
                break;
            default:
                throw new ArgumentOutOfRangeException($"{nameof(ESide)} enum does not contain the given value on {nameof(AddConnectionPin)} method.");
        }

        _connectionsCanvas!.Children.Add(pin);
        _UpdateConnectionPins();
    }

    private void _UpdateConnectionPins()
    {
        for (int i = 0; i < m_leftConnectionPins.Count; i++)
        {
            var pin = m_leftConnectionPins[i];
            double leftPosition = pin.Width / 2.0d;
            double bottomPosition = pin.Height * (i + 1);
            Canvas.SetLeft(pin, leftPosition);
            Canvas.SetBottom(pin, bottomPosition);
        }

        for (int i = 0; i < m_rightConnectionPins.Count; i++)
        {
            var pin = m_rightConnectionPins[i];
            double rightPosition = pin.Width / 2.0d;
            double bottomPosition = pin.Height * (i + 1);
            Canvas.SetRight(pin, rightPosition);
            Canvas.SetBottom(pin, bottomPosition);
        }

        for (int i = 0; i < m_topConnectionPins.Count; i++)
        {
            var pin = m_topConnectionPins[i];
            double topPosition = pin.Height / 2.0d;
            double leftPosition = pin.Width * (i + 1);
            Canvas.SetTop(pin, topPosition);
            Canvas.SetLeft(pin, leftPosition);
        }

        for (int i = 0; i < m_bottomConnectionPins.Count; i++)
        {
            var pin = m_bottomConnectionPins[i];
            double bottomPosition = pin.Height / 2.0d;
            double leftPosition = pin.Width * (i + 1);
            Canvas.SetBottom(pin, bottomPosition);
            Canvas.SetLeft(pin, leftPosition);
        }
    }

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
