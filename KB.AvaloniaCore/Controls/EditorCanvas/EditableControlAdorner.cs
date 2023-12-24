using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.VisualTree;
using KB.AvaloniaCore.Injection;
using KB.AvaloniaCore.Utils;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Adorner to edit <see cref="IEditableControl"/>s."/>
/// It can move, resize and rotate the adorned controls.
/// </summary>
[TemplatePart("PART_LeftTopThumb", typeof(Thumb))]
[TemplatePart("PART_LeftBottomThumb", typeof(Thumb))]
[TemplatePart("PART_RightTopThumb", typeof(Thumb))]
[TemplatePart("PART_RightBottomThumb", typeof(Thumb))]
internal class EditableControlAdorner : TemplatedControl
{
    private Point _previousPosition;
    private readonly Control _host;
    private AdornerLayer? _adornerLayer;

    private Thumb? _leftTopThumb;
    private Thumb? _leftBottomThumb;
    private Thumb? _rightTopThumb;
    private Thumb? _rightBottomThumb;

    private readonly ViewCursorHolder _cursorHolder;

    #region Values before editing

    private Point[]? _oldPositions;
    private double[]? _oldWidths;
    private double[]? _oldHeights;

    #endregion

    /// <summary>
    /// Used to calculate the correct delta. Because the delta is the delta of the thumb and not the delta of the element that is being dragged and scaled.
    /// </summary>
    private Point _previousDeltaPositionChangeOnThumbDelta;

    static EditableControlAdorner()
    {
        EditableControlAdorner.AdornedElementsProperty.Changed.AddClassHandler<EditableControlAdorner>((s, e) => s._OnAdornedElementsPropertyChanged(e));
    }


    public EditableControlAdorner(Control host)
    {
        _host = host ?? throw new ArgumentException($"{nameof(host)} can't be null.");
        _host.AttachedToVisualTree += _OnHostAttachedToVisualTree;
        _host.DetachedFromVisualTree += _OnHostDetachedFromVisualTree;
        AdornerLayer.SetAdornedElement(this, _host);
        _previousDeltaPositionChangeOnThumbDelta = default(Point);
        _cursorHolder = new ViewCursorHolder(this);
        _oldPositions = null;
        _oldWidths = null;
        _oldHeights = null;
    }

    private void _OnHostAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // If we come from a detached state, we have to activate the adorner
        if (AdornedElements != null)
        {
            Activate();
        }
    }

    private void _OnHostDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Deactivate();
    }

    /// <summary>
    /// Event raised when the adorned elements finished moving.
    /// Argument is the old positions of the elements.
    /// </summary>
    public event Action<Point[]>? OnAdornedElementsMoveFinished;
    /// <summary>
    /// Event raised when the adorned elements finished scaling.
    /// Argument is the old sizes of the elements. Widht and Height.
    /// </summary>
    public event Action<double[], double[]>? OnAdornedElementsScaleFinished; 

    public bool IsDraggingElements { get; private set; }
    public bool IsScalingElements { get; private set; }

    public bool IsActive { get; private set; }

    public static readonly StyledProperty<IEnumerable<IEditableControl>?> AdornedElementsProperty = AvaloniaProperty.Register<EditableControlAdorner, IEnumerable<IEditableControl>?>(nameof(EditableControlAdorner.AdornedElements));
    public static readonly StyledProperty<bool> CanResizeProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanResize), defaultValue: true);
    public static readonly StyledProperty<bool> CanMoveProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanMove), defaultValue: true);
    public static readonly StyledProperty<IControlTemplate> ScaleThumbTemplateProperty = AvaloniaProperty.Register<EditableControlAdorner, IControlTemplate>(nameof(EditableControlAdorner.ScaleThumbTemplate));

    public IEnumerable<IEditableControl>? AdornedElements
    {
        get { return GetValue(AdornedElementsProperty); }
        set { SetValue(AdornedElementsProperty, value); }
    }

    public bool CanResize
    {
        get { return GetValue(CanResizeProperty); }
        set { SetValue(CanResizeProperty, value); }
    }

    public bool CanMove
    {
        get { return GetValue(CanMoveProperty); }
        set { SetValue(CanMoveProperty, value); }
    }

    public IControlTemplate ScaleThumbTemplate
    {
        get { return GetValue(ScaleThumbTemplateProperty); }
        set { SetValue(ScaleThumbTemplateProperty, value); }
    }

    public bool IsPointOver(Point point)
    {
        return CanvasExtension.IsPointOverCanvasChild(point, _host);
    }


    private void _OnAdornedElementsPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _MeasureHost();

        if(e.OldValue is INotifyCollectionChanged oldAdornedElements)
        {
            oldAdornedElements.CollectionChanged -= _OnAdornedElementsCollectionChanged;
            _UnsubscribeToElementChanges((IEnumerable<IEditableControl>)e.OldValue);
        }

        if(e.NewValue is INotifyCollectionChanged newAdornedElements)
        {
            newAdornedElements.CollectionChanged += _OnAdornedElementsCollectionChanged;
            _SubscribeToElementChanges((IEnumerable<IEditableControl>)e.NewValue);
        }
    }

    private void _UnsubscribeToElementChanges(IEnumerable<IEditableControl> elements)
    {
        foreach (IEditableControl element in elements)
        {
            element.PositionXChanged -= _OnAdornedElementMeasureChanged;
            element.PositionYChanged -= _OnAdornedElementMeasureChanged;
            element.WidthChanged -= _OnAdornedElementMeasureChanged;
            element.HeightChanged -= _OnAdornedElementMeasureChanged;
        }
    }

    private void _SubscribeToElementChanges(IEnumerable<IEditableControl> elements)
    {
        foreach (IEditableControl element in elements)
        {
            element.PositionXChanged += _OnAdornedElementMeasureChanged;
            element.PositionYChanged += _OnAdornedElementMeasureChanged;
            element.WidthChanged += _OnAdornedElementMeasureChanged;
            element.HeightChanged += _OnAdornedElementMeasureChanged;
        }
    }

    private void _OnAdornedElementMeasureChanged(object? sender, EventArgs e)
    {
        if(!IsDraggingElements)
        {
            // Position changed from outside the adorner
            _MeasureHost();
        }
    }

    private void _OnAdornedElementsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if(e.OldItems != null)
        {
            _UnsubscribeToElementChanges(e.OldItems.Cast<IEditableControl>());
        }

        if(e.NewItems != null)
        {
            _SubscribeToElementChanges(e.NewItems.Cast<IEditableControl>());
        }

        _MeasureHost();
    }

    public void Activate()
    {
        if(IsActive)
        {
            return;
        }

        IsActive = true;
        _adornerLayer = AdornerLayer.GetAdornerLayer(_host.FindAncestorOfType<Canvas>()!)!;
        _adornerLayer.Children.Add(this);

        if (AdornedElements == null)
        {
            return;
        }

        // Set the size of the host the same as the size of the adorned elements
        _MeasureHost();
        IsDraggingElements = true;
    }

    public void Deactivate()
    {
        IsDraggingElements = false;
        if(!IsActive)
        {
            return;
        }

        IsActive = false;
        _adornerLayer!.Children.Remove(this);
    }

    public void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(e.Handled || !IsActive)
        {
            return;
        }

        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if (e.ClickCount == 1 && properties.IsLeftButtonPressed)
        {
            IsDraggingElements = true;
            _previousPosition = e.GetPosition(this);
            e.Handled = true;
        }
    }

    public void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (IsDraggingElements)
        {
            if(_oldPositions == null)
            {
                _oldPositions = AdornedElements!.Select(x => new Point(x.PositionX, x.PositionY)).ToArray();
            }

            _MoveElements(e.GetPosition(this));
            e.Handled = true;
        }
    }

    public void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if(IsDraggingElements)
        {
            if(_oldPositions != null)
            {
                OnAdornedElementsMoveFinished?.Invoke(_oldPositions!);
                _oldPositions = null;
            }

            IsDraggingElements = false;
        }

        e.Handled = true;
    }

    #region ThumbInteraction

    private void _OnScaleThumbLeftTopDragDelta(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (CanResize)
        {
            _StartScaleElements();
            double deltaX = e.Vector.X - _previousDeltaPositionChangeOnThumbDelta.X;
            double deltaY = e.Vector.Y - _previousDeltaPositionChangeOnThumbDelta.Y;
            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            double currentHostPositionX = Canvas.GetLeft(_host!);
            double currentHostPositionY = Canvas.GetTop(_host!);
            double newHostPositionX = currentHostPositionX + deltaX;
            double newHostPositionY = currentHostPositionY + deltaY;
            double newHostWidth = _host.Width - deltaX;
            double newHostHeight = _host.Height - deltaY;

            _ScaleRelativeToHost(newHostPositionX, newHostPositionY, newHostWidth, newHostHeight);
        }

        e.Handled = true;
    }

    private void _OnScaleThumbLeftBottomDragDelta(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (CanResize)
        {
            _StartScaleElements();
            double deltaX = e.Vector.X - _previousDeltaPositionChangeOnThumbDelta.X;
            double deltaY = e.Vector.Y;
            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            double currentHostPositionX = Canvas.GetLeft(_host!);
            double currentHostPositionY = Canvas.GetTop(_host!);

            double newHostPositionX = currentHostPositionX + deltaX;
            double newHostPositionY = currentHostPositionY;

            double newHostWidth = _host.Width - deltaX;
            double newHostHeight = _host.Height + deltaY;

            _ScaleRelativeToHost(newHostPositionX, newHostPositionY, newHostWidth, newHostHeight);
        }

        e.Handled = true;
    }

    private void _OnScaleThumbRightTopDragDelta(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (CanResize)
        {
            _StartScaleElements();
            double deltaX = e.Vector.X;
            double deltaY = e.Vector.Y - _previousDeltaPositionChangeOnThumbDelta.Y;
            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            double currentHostPositionX = Canvas.GetLeft(_host!);
            double currentHostPositionY = Canvas.GetTop(_host!);

            double newHostPositionX = currentHostPositionX;
            double newHostPositionY = currentHostPositionY + deltaY;

            double newHostWidth = _host.Width + deltaX;
            double newHostHeight = _host.Height - deltaY;

            _ScaleRelativeToHost(newHostPositionX, newHostPositionY, newHostWidth, newHostHeight);
        }

        e.Handled = true;
    }

    private void _OnScaleThumbRightBottomDragDelta(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (CanResize)
        {
            _StartScaleElements();
            double deltaX = e.Vector.X;
            double deltaY = e.Vector.Y;

            double currentHostPositionX = Canvas.GetLeft(_host!);
            double currentHostPositionY = Canvas.GetTop(_host!);

            double newHostPositionX = currentHostPositionX;
            double newHostPositionY = currentHostPositionY;

            double newHostWidth = _host.Width + deltaX;
            double newHostHeight = _host.Height + deltaY;

            _ScaleRelativeToHost(newHostPositionX, newHostPositionY, newHostWidth, newHostHeight);
        }

        e.Handled = true;
    }

    private void _StartScaleElements()
    {
        IsScalingElements = true;
        if(_oldWidths == null || _oldHeights == null)
        {
            _oldWidths = AdornedElements!.Select(x => x.Width).ToArray();
            _oldHeights = AdornedElements!.Select(x => x.Height).ToArray();
        }
    }

    private void _OnThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }
        
        if(IsScalingElements)
        {
            IsScalingElements = false;
            if(_oldWidths != null && _oldHeights != null)
            {
                OnAdornedElementsScaleFinished?.Invoke(_oldWidths!, _oldHeights!);
                _oldWidths = null;
                _oldHeights = null;
            }

            _previousDeltaPositionChangeOnThumbDelta = default(Point);
            e.Handled = true;
        }
    }

    private void _OnThumbPointerEnter(object? sender, PointerEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (sender is Thumb thumb)
        {
            switch(thumb.Name)
            {
                case "PART_LeftTopThumb":
                {
                    _cursorHolder.SetCursor(CursorManager.Instance.CursorTopLeftCorner);
                    break;
                }
                case "PART_LeftBottomThumb":
                {
                    _cursorHolder.SetCursor(CursorManager.Instance.CursorBottomLeftCorner);
                    break;
                }
                case "PART_RightTopThumb":
                {
                    _cursorHolder.SetCursor(CursorManager.Instance.CursorTopRightCorner);
                    break;
                }
                case "PART_RightBottomThumb":
                {
                    _cursorHolder.SetCursor(CursorManager.Instance.CursorBottomRightCorner);
                    break;
                }
            }
        }

        e.Handled = true;
    }

    private void _OnThumbPointerExited(object? sender, PointerEventArgs e)
    {
        _cursorHolder.ReturnToPreviousCursor();
    }

    #endregion

    #region Inherited Members

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _leftTopThumb = e.NameScope.Get<Thumb>("PART_LeftTopThumb");
        _leftBottomThumb = e.NameScope.Get<Thumb>("PART_LeftBottomThumb");
        _rightTopThumb = e.NameScope.Get<Thumb>("PART_RightTopThumb");
        _rightBottomThumb = e.NameScope.Get<Thumb>("PART_RightBottomThumb");

        if (ScaleThumbTemplate != null)
        {
            _leftTopThumb!.Template = ScaleThumbTemplate;
            _leftBottomThumb!.Template = ScaleThumbTemplate;
            _rightTopThumb!.Template = ScaleThumbTemplate;
            _rightBottomThumb!.Template = ScaleThumbTemplate;
        }

        // Pointer Entered
        _leftTopThumb!.PointerEntered += _OnThumbPointerEnter;
        _leftBottomThumb!.PointerEntered += _OnThumbPointerEnter;
        _rightTopThumb!.PointerEntered += _OnThumbPointerEnter;
        _rightBottomThumb!.PointerEntered += _OnThumbPointerEnter;

        // Pointer Exited
        _leftTopThumb!.PointerExited += _OnThumbPointerExited;
        _leftBottomThumb!.PointerExited += _OnThumbPointerExited;
        _rightTopThumb!.PointerExited += _OnThumbPointerExited;
        _rightBottomThumb!.PointerExited += _OnThumbPointerExited;
            
        // Delta scale
        _leftTopThumb!.DragDelta += _OnScaleThumbLeftTopDragDelta;
        _leftBottomThumb!.DragDelta += _OnScaleThumbLeftBottomDragDelta;
        _rightTopThumb!.DragDelta += _OnScaleThumbRightTopDragDelta;
        _rightBottomThumb!.DragDelta += _OnScaleThumbRightBottomDragDelta;

        // Drag completed
        _leftBottomThumb!.DragCompleted += _OnThumbDragCompleted;
        _rightBottomThumb!.DragCompleted += _OnThumbDragCompleted;
        _leftTopThumb!.DragCompleted += _OnThumbDragCompleted;
        _rightTopThumb!.DragCompleted += _OnThumbDragCompleted;
    }

    #endregion

    private void _MoveElements(Point currentPosition)
    {
        Vector delta = currentPosition - _previousPosition;
        _previousPosition = currentPosition;

        double newLeft = Canvas.GetLeft(_host!) + delta.X;
        double newTop = Canvas.GetTop(_host!) + delta.Y;
        Canvas.SetLeft(_host!, newLeft);
        Canvas.SetTop(_host!, newTop);

        // Update editable controls
        foreach (IEditableControl editableControl in AdornedElements!)
        {
            if (CanMove)
            {
                editableControl.PositionX += delta.X;
                editableControl.PositionY += delta.Y;
            }
        }

    }

    private void _MeasureHost()
    {
        if(!IsActive)
        {
            return;
        }

        // Calcualte the size of the adorner
        Size size = new Size();
        // Calculate the center of the AdornedElements
        // Then reposition the host so the center of the host is the same as the center of the AdornedElements
        double adornersMinPositionX = Double.MaxValue;
        double adornersMinPositionY = Double.MaxValue;

        if (AdornedElements != null)
        {
            // Calculate the position of the adorner based on the adorned element. It can have multiple elements so it has to take into account all of them.
            //
            //- *********************************** +
            //|(0, 0)                 (max x, min y)|
            //|                                     |
            //|                                     |
            //|                                     |
            //|                                     |
            //|                                     |
            //|                                     |
            //|(min x, max y)         (max x, max y)|
            //- *********************************** +

            // Calculate the size of the adorner based on the adorned element. It can have multiple elements so it has to take into account all of them.
            foreach (IEditableControl editableControl in AdornedElements)
            {
                size = new Size(System.Math.Max(size.Width, editableControl.Width + editableControl.PositionX), System.Math.Max(size.Height, editableControl.Height + editableControl.PositionY));
                adornersMinPositionX = System.Math.Min(adornersMinPositionX, editableControl.PositionX);
                adornersMinPositionY = System.Math.Min(adornersMinPositionY, editableControl.PositionY);
            }
        }

        size = new Size(size.Width - adornersMinPositionX, size.Height - adornersMinPositionY);
        _host!.Width = size.Width;
        _host!.Height = size.Height;

        Canvas.SetLeft(_host, adornersMinPositionX);
        Canvas.SetTop(_host, adornersMinPositionY);
    }

    private void _ScaleRelativeToHost(double newHostPositionX, double newHostPositionY, double newHostWidth, double newHostHeight)
    {
        double miniumMeasureFactor = 1.8d;
        double miniumWidth = _leftTopThumb!.Bounds.Width * miniumMeasureFactor * 2 * AdornedElements!.Count();
        double miniumHeight = _leftTopThumb!.Bounds.Height * miniumMeasureFactor * 2 * AdornedElements!.Count();
        bool isValidWidth = newHostWidth >= miniumWidth;
        bool isValidHeight = newHostHeight >= miniumHeight;

        if(!isValidWidth && !isValidHeight)
        {
            return;
        }

        double currentHostPositionX = Canvas.GetLeft(_host!);
        double currentHostPositionY = Canvas.GetTop(_host!);

        double changePercentageWidth = newHostWidth / _host.Width; ;
        double changePercentageHeight = newHostHeight / _host.Height;

        // Controls have to be in the same relative position to the host as before the resize
        foreach (IEditableControl editableControl in AdornedElements!)
        {
            if(isValidWidth)
            {
                double oldRelativePositionX = editableControl.PositionX - currentHostPositionX;
                double newRelativePositionX = oldRelativePositionX * changePercentageWidth;
                editableControl.PositionX = newHostPositionX + newRelativePositionX;
                editableControl.Width *= changePercentageWidth;
            }

            if(isValidHeight)
            {
                double oldRelativePositionY = editableControl.PositionY - currentHostPositionY;
                double newRelativePositionY = oldRelativePositionY * changePercentageHeight;
                editableControl.PositionY = newHostPositionY + newRelativePositionY;
                editableControl.Height *= changePercentageHeight;
            }
        }

        if(isValidWidth)
        {
            _host!.Width = newHostWidth;
            Canvas.SetLeft(_host, newHostPositionX);
        }

        if(isValidHeight)
        {
            _host!.Height = newHostHeight;
            Canvas.SetTop(_host, newHostPositionY);
        }
    }
}
