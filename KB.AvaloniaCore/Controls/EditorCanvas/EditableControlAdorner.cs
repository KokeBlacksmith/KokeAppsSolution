using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Draggable;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Adorner to edit <see cref="IEditableControl"/>s."/>
/// It can move, resize and rotate the adorned controls.
/// </summary>
[TemplatePart("PART_LeftTopThumb", typeof(Thumb))]
[TemplatePart("PART_LeftBottomThumb", typeof(Thumb))]
[TemplatePart("PART_RightTopThumb", typeof(Thumb))]
[TemplatePart("PART_RightBottomThumb", typeof(Thumb))]
[TemplatePart("PART_RotateThumb", typeof(Thumb))]
internal class EditableControlAdorner : TemplatedControl
{
    private bool _isDraggingElements;
    private Point _previousPosition;
    private readonly Control _host;

    private Thumb? _leftTopThumb;
    private Thumb? _leftBottomThumb;
    private Thumb? _rightTopThumb;
    private Thumb? _rightBottomThumb;
    private Thumb? _rotateThumb;

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
        //IsHitTestVisible = false;
        _isDraggingElements = false;
        _host = host;
        AdornerLayer.SetAdornedElement(this, _host);
        _previousDeltaPositionChangeOnThumbDelta = default(Point);
    }

    public bool IsDraggingElements => _isDraggingElements;
    public bool IsActive { get; private set; }

    public static readonly StyledProperty<IEnumerable<IEditableControl>?> AdornedElementsProperty = AvaloniaProperty.Register<EditableControlAdorner, IEnumerable<IEditableControl>?>(nameof(EditableControlAdorner.AdornedElements));
    public static readonly StyledProperty<bool> CanRotateProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanRotate), defaultValue: true);
    public static readonly StyledProperty<bool> CanResizeProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanResize), defaultValue: true);
    public static readonly StyledProperty<bool> CanMoveProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanMove), defaultValue: true);
    public static readonly StyledProperty<IControlTemplate> ScaleThumbTemplateProperty = AvaloniaProperty.Register<EditableControlAdorner, IControlTemplate>(nameof(EditableControlAdorner.ScaleThumbTemplate));
    public static readonly StyledProperty<IControlTemplate> RotateThumbTemplateProperty = AvaloniaProperty.Register<EditableControlAdorner, IControlTemplate>(nameof(EditableControlAdorner.RotateThumbTemplate));

    public IEnumerable<IEditableControl>? AdornedElements
    {
        get { return GetValue(AdornedElementsProperty); }
        set { SetValue(AdornedElementsProperty, value); }
    }

    public bool CanRotate
    {
        get { return GetValue(CanRotateProperty); }
        set { SetValue(CanRotateProperty, value); }
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

    public IControlTemplate RotateThumbTemplate
    {
        get { return GetValue(RotateThumbTemplateProperty); }
        set { SetValue(RotateThumbTemplateProperty, value); }
    }


    private void _OnAdornedElementsPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _MeasureHost();

        if(e.OldValue is INotifyCollectionChanged oldAdornedElements)
        {
            oldAdornedElements.CollectionChanged -= _OnAdornedElementsCollectionChanged;
        }

        if(e.NewValue is INotifyCollectionChanged newAdornedElements)
        {
            newAdornedElements.CollectionChanged += _OnAdornedElementsCollectionChanged;
        }
    }

    private void _OnAdornedElementsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _MeasureHost();
    }

    public void Activate()
    {
        if(IsActive)
        {
            return;
        }

        IsActive = true;
        AdornerLayer layer = AdornerLayer.GetAdornerLayer(_host.FindAncestorOfType<Canvas>()!)!;
        layer.Children.Add(this);

        if (AdornedElements == null)
        {
            return;
        }

        // Set the size of the host the same as the size of the adorned elements
        _MeasureHost();
        _isDraggingElements = true;
    }

    public void Deactivate()
    {
        _isDraggingElements = false;
        if(!IsActive)
        {
            return;
        }

        IsActive = false;
        AdornerLayer layer = AdornerLayer.GetAdornerLayer(_host.FindAncestorOfType<Canvas>()!)!;
        layer.Children.Remove(this);
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
            _isDraggingElements = true;
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

        if (_isDraggingElements)
        {
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
        
        _isDraggingElements = false;
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
            double deltaX = e.Vector.X - _previousDeltaPositionChangeOnThumbDelta.X;
            double deltaY = e.Vector.Y - _previousDeltaPositionChangeOnThumbDelta.Y;

            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            foreach (IEditableControl editableControl in AdornedElements!)
            {
                editableControl.PositionX += deltaX;
                editableControl.PositionY += deltaY;

                editableControl.Width -= deltaX;
                editableControl.Height -= deltaY;
            }
        }

        _MeasureHost();
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
            double deltaX = e.Vector.X - _previousDeltaPositionChangeOnThumbDelta.X;
            double deltaY = e.Vector.Y;

            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            foreach (IEditableControl editableControl in AdornedElements!)
            {
                editableControl.PositionX += deltaX;

                editableControl.Width -= deltaX;
                editableControl.Height += deltaY;
            }
        }

        _MeasureHost();
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
            double deltaX = e.Vector.X;
            double deltaY = e.Vector.Y - _previousDeltaPositionChangeOnThumbDelta.Y;

            _previousDeltaPositionChangeOnThumbDelta = new Point(e.Vector.X, e.Vector.Y);

            foreach (IEditableControl editableControl in AdornedElements!)
            {
                editableControl.PositionY += deltaY;

                editableControl.Width += deltaX;
                editableControl.Height -= deltaY;
            }
        }

        _MeasureHost();
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
            double deltaX = e.Vector.X;
            double deltaY = e.Vector.Y;

            foreach (IEditableControl editableControl in AdornedElements!)
            {
                editableControl.Width += deltaX;
                editableControl.Height += deltaY;
            }
        }

        _MeasureHost();
        e.Handled = true;
    }

    private void _OnRotateThumbDragDelta(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        if (CanRotate)
        {
            foreach (IEditableControl editableControl in AdornedElements!)
            {
                //editableControl.Rotation += e.Vector.X;
            }
        }

        _MeasureHost();
        e.Handled = true;
    }

    private void _OnRotateThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        if (e.Handled || !IsActive)
        {
            return;
        }

        _previousDeltaPositionChangeOnThumbDelta = default(Point);
        e.Handled = true;
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
        _rotateThumb = e.NameScope.Get<Thumb>("PART_RotateThumb");

        if (ScaleThumbTemplate != null)
        {
            _leftTopThumb!.Template = ScaleThumbTemplate;
            _leftBottomThumb!.Template = ScaleThumbTemplate;
            _rightTopThumb!.Template = ScaleThumbTemplate;
            _rightBottomThumb!.Template = ScaleThumbTemplate;
        }

        if (RotateThumbTemplate != null)
        {
            _rotateThumb!.Template = RotateThumbTemplate;
        }

        _leftTopThumb!.DragDelta += _OnScaleThumbLeftTopDragDelta;
        _leftBottomThumb!.DragDelta += _OnScaleThumbLeftBottomDragDelta;
        _rightTopThumb!.DragDelta += _OnScaleThumbRightTopDragDelta;
        _rightBottomThumb!.DragDelta += _OnScaleThumbRightBottomDragDelta;
        _rotateThumb!.DragDelta += _OnRotateThumbDragDelta;

        _leftBottomThumb!.DragCompleted += _OnRotateThumbDragCompleted;
        _rightBottomThumb!.DragCompleted += _OnRotateThumbDragCompleted;
        _leftTopThumb!.DragCompleted += _OnRotateThumbDragCompleted;
        _rightTopThumb!.DragCompleted += _OnRotateThumbDragCompleted;
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
}
