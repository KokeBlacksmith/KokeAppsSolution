using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using KB.AvaloniaCore.Controls.GraphEditor.Events;
using KB.AvaloniaCore.Injection;
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
    private readonly Canvas? _pinsCanvas;

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

        _pinsCanvas = new Canvas()
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
        _mainPanel.Children.Add(_pinsCanvas);

        ((ISetLogicalParent)_mainPanel).SetParent(this);
        VisualChildren.Add(_mainPanel);
        LogicalChildren.Add(_mainPanel);
    }


    #region Methods

    public void AddConnectionPin(NodePin pin, ESide side)
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

        _pinsCanvas!.Children.Add(pin);
        //pin.AddHandler(PointerPressedEvent, _OnPointerPressedOverNodePin, RoutingStrategies.Direct);
        //pin.AddHandler(PointerReleasedEvent, _OnPointerReleasedOverNodePin, RoutingStrategies.Direct);
        pin.AddHandler(PointerPressedEvent, _OnPointerPressedOverNodePin);
        pin.AddHandler(PointerReleasedEvent, _OnPointerReleasedOverNodePin);
        pin.AddHandler(PointerMovedEvent, _OnPointerMovedOverNodePin);
        m_RepositionConnectionPins();
    }

    public void RemoveConnectionPin(NodePin pin, ESide side)
    {
        switch (side)
        {
            case ESide.Left:
                m_leftConnectionPins.Remove(pin);
                break;
            case ESide.Right:
                m_rightConnectionPins.Remove(pin);
                break;
            case ESide.Top:
                m_topConnectionPins.Remove(pin);
                break;
            case ESide.Bottom:
                m_bottomConnectionPins.Remove(pin);
                break;
            default:
                throw new ArgumentOutOfRangeException($"{nameof(ESide)} enum does not contain the given value on {nameof(RemoveConnectionPin)} method.");
        }

        _pinsCanvas!.Children.Remove(pin);
        pin.RemoveHandler(PointerPressedEvent, _OnPointerPressedOverNodePin);
        pin.RemoveHandler(PointerReleasedEvent, _OnPointerReleasedOverNodePin);
        pin.RemoveHandler(PointerMovedEvent, _OnPointerMovedOverNodePin);
        m_RepositionConnectionPins();
    }

    private void _OnPointerPressedOverNodePin(object? sender, PointerPressedEventArgs e)
    {
        // Get the pin
        NodePin pin = ((Visual)e.Source!).GetParentOfTypeIncludeSelf<NodePin>();   
        ConnectionPinPressed?.Invoke(pin, new NodePinPointerInteractionEventArgs(pin, e.GetPosition(this)));
        e.Handled = true;
    }

    private void _OnPointerReleasedOverNodePin(object? sender, PointerReleasedEventArgs e)
    {
        // Get the pin
        NodePin pin = ((Visual)e.Source!).GetParentOfTypeIncludeSelf<NodePin>();
        ConnectionPinReleased?.Invoke(pin, new NodePinPointerInteractionEventArgs(pin, e.GetPosition(this)));
        e.Handled = true;
    }

    private void _OnPointerMovedOverNodePin(object? sender, PointerEventArgs e)
    {
        // Get the pin
        NodePin pin = ((Visual)e.Source!).GetParentOfTypeIncludeSelf<NodePin>();
        ConnectionPinPointerMoved?.Invoke(pin, new NodePinPointerInteractionEventArgs(pin, e.GetPosition(this)));
        e.Handled = true;
    }

    protected override void OnMeasureInvalidated()
    {
        base.OnMeasureInvalidated();
        m_RepositionConnectionPins();
    }

    /// <summary>
    /// Reposition pins
    /// Pins must be set using <see cref="Canvas.SetLeft"/> and <see cref="Canvas.SetTop"/>
    /// </summary>
    protected virtual void m_RepositionConnectionPins()
    {
        // Positioning the pins.
        // It starts from the center and goes outwards to both sides.

        if(Double.IsNaN(Width) || Double.IsNaN(Height))
        {
            return;
        }

        double centerX = this.GetHalfWidth();
        double centerY = this.GetHalfHeight();
        double marginRatio = 0.8d;

        // Left pins
        int leftPinsCount = m_leftConnectionPins.Count;
        if(leftPinsCount > 0)
        {
            double leftPinSeparation = (Height * marginRatio) / leftPinsCount;
            // Place pins top to bottom
            for (int i = leftPinsCount - 1; i >= 0; --i)
            {
                NodePin pin = m_leftConnectionPins[i];

                double leftPosition = pin.GetHalfWidth() * -1.0d;
                Canvas.SetLeft(pin, leftPosition);

                double topPosition;
                if(leftPinsCount == 1)
                {
                    topPosition = centerY;
                }
                else
                {
                    // Distribute the pins evenly
                    topPosition = Height - leftPinSeparation * (i + 1);
                }

                Canvas.SetTop(pin, topPosition);
            }
        }

        // Right pins
        int rightPinsCount = m_rightConnectionPins.Count;
        if(rightPinsCount > 0)
        {
            double rightPinSeparation = (Height * marginRatio) / rightPinsCount;
            // Place pins top to bottom
            for (int i = rightPinsCount - 1; i >= 0; --i)
            {
                var pin = m_rightConnectionPins[i];
                double rightPosition = pin.GetHalfWidth() * -1.0d;
                Canvas.SetLeft(pin, Width + rightPosition);

                double topPosition;
                if (rightPinsCount == 1)
                {
                    topPosition = centerY;
                }
                else
                {
                    // Distribute the pins evenly
                    topPosition = Height - rightPinSeparation * (i + 1);
                }
                
                Canvas.SetBottom(pin, topPosition);
            }
        }

        // Top pins
        int topPinsCount = m_topConnectionPins.Count;
        if (topPinsCount > 0)
        {
            double topPinSeparation = (Width * marginRatio) / topPinsCount;
            // Place pins left to right
            for (int i = 0; i < m_topConnectionPins.Count; i++)
            {
                var pin = m_topConnectionPins[i];
                double topPosition = pin.GetHalfHeight() * -1.0d;
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
            double bottomPinSeparation = (Width * marginRatio) / bottomPinsCount;
            // Place pins left to right
            for (int i = 0; i < bottomPinsCount; ++i)
            {
                var pin = m_bottomConnectionPins[i];
                double bottomPosition = pin.GetHalfHeight() * -1.0d;
                Canvas.SetTop(pin, Height + bottomPosition);

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
