using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using KB.SharpCore.Enums;
using KB.SharpCore.Events;

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

        m_RepositionConnectionPins();
    }

    /// <summary>
    /// Reposition pins
    /// </summary>
    protected virtual void m_RepositionConnectionPins()
    {
        // Positioning the pins.
        // It starts from the center and goes outwards to both sides.

        if(Double.IsNaN(Width) || Double.IsNaN(Height))
        {
            return;
        }

        double centerX = Width / 2.0d;
        double centerY = Height / 2.0d;
        double marginErrorRatio = 0.8d;

        // Left pins
        int leftPinsCount = m_leftConnectionPins.Count;
        if(leftPinsCount > 0)
        {
            double leftPinSeparation = (Height * marginErrorRatio) / leftPinsCount;
            for (int i = 0; i < leftPinsCount; ++i)
            {
                NodeConnectionPin pin = m_leftConnectionPins[i];

                double leftPosition = (pin.Width / 2.0d) * -1.0d;
                Canvas.SetLeft(pin, leftPosition);

                double bottomPosition;
                if(leftPinsCount == 1)
                {
                    bottomPosition = centerY;
                }
                else
                {
                    // Distribute the pins evenly
                    bottomPosition = leftPinSeparation * (i + 1);
                }

                Canvas.SetBottom(pin, bottomPosition);
            }
        }

        // Right pins
        int rightPinsCount = m_rightConnectionPins.Count;
        if(rightPinsCount > 0)
        {
            double rightPinSeparation = (Height * marginErrorRatio) / rightPinsCount;
            for (int i = 0; i < rightPinsCount; i++)
            {
                var pin = m_rightConnectionPins[i];
                double rightPosition = (pin.Width / 2.0d) * -1.0d;
                Canvas.SetRight(pin, rightPosition);

                double bottomPosition;
                if (rightPinsCount == 1)
                {
                    bottomPosition = centerY;
                }
                else
                {
                    // Distribute the pins evenly
                    bottomPosition = rightPinSeparation * (i + 1);
                }
                
                Canvas.SetBottom(pin, bottomPosition);
            }
        }

        // Top pins
        int topPinsCount = m_topConnectionPins.Count;
        if (topPinsCount > 0)
        {
            double topPinSeparation = (Width * marginErrorRatio) / topPinsCount;
            for (int i = 0; i < m_topConnectionPins.Count; i++)
            {
                var pin = m_topConnectionPins[i];
                double topPosition = (pin.Height / 2.0d) * -1.0d;
                Canvas.SetTop(pin, topPosition);

                double leftPosition;
                if (topPinsCount == 1)
                {
                    leftPosition = centerX;
                }
                else
                {
                    // Distribute the pins evenly
                    leftPosition = topPinSeparation * (i + 1);
                }

                Canvas.SetLeft(pin, leftPosition);
            }
        }

        // Bottom pins
        int bottomPinsCount = m_bottomConnectionPins.Count;
        if (bottomPinsCount > 0)
        {
            double bottomPinSeparation = (Width * marginErrorRatio) / bottomPinsCount;
            for (int i = 0; i < bottomPinsCount; ++i)
            {
                var pin = m_bottomConnectionPins[i];
                double bottomPosition = (pin.Height / 2.0d) * -1.0d;
                Canvas.SetBottom(pin, bottomPosition);

                double leftPosition;
                if (bottomPinsCount == 1)
                {
                    leftPosition = centerX;
                }
                else
                {
                    // Distribute the pins evenly
                    leftPosition = bottomPinSeparation * (i + 1);
                }

                Canvas.SetLeft(pin, leftPosition);
            }
        }
    }

    #endregion

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        m_RepositionConnectionPins();
    }

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

    private void _OnPositionXPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ValueChangedEventArgs<double> args = new ValueChangedEventArgs<double>((double)e.OldValue!, (double)e.NewValue!);
        PositionXChanged?.Invoke(this, args);
    }
    private void _OnPositionYPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ValueChangedEventArgs<double> args = new ValueChangedEventArgs<double>((double)e.OldValue!, (double)e.NewValue!);
        PositionYChanged?.Invoke(this, args);
    }

    private void _OnWidthPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ValueChangedEventArgs<double> args = new ValueChangedEventArgs<double>((double)e.OldValue!, (double)e.NewValue!);
        WidthChanged?.Invoke(this, args);
    }

    private void _OnHeightPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ValueChangedEventArgs<double> args = new ValueChangedEventArgs<double>((double)e.OldValue!, (double)e.NewValue!);
        HeightChanged?.Invoke(this, args);
    }

    private void _OnIsSelectedPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ValueChangedEventArgs<bool> args = new ValueChangedEventArgs<bool>((bool)e.OldValue!, (bool)e.NewValue!);
        IsSelectedChanged?.Invoke(this, args);
    }

    #endregion
}
