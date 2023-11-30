using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

//https://github.com/wieslawsoltes/PerspectiveDemo/blob/Resizing/PerspectiveDemo/SelectedAdorner.axaml.cs
//https://github.com/wieslawsoltes/PerspectiveDemo/blob/Resizing/PerspectiveDemo/SelectedAdorner.axaml

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Adorner to edit <see cref="IEditableControl"/>s."/>
/// It can move, resize and rotate the adorned controls.
/// </summary>
internal class EditableControlAdorner : TemplatedControl
{
    private bool _isDraggingElements;
    private Point _previousPosition;
    private readonly Control _host;
    private bool _isActive;

    static EditableControlAdorner()
    {
        EditableControlAdorner.AdornedElementsProperty.Changed.AddClassHandler<EditableControlAdorner>((s, e) => s._OnAdornedElementsPropertyChanged(e));
    }


    public EditableControlAdorner(Control host)
    {
        IsHitTestVisible = false;
        _isDraggingElements = false;
        _isActive = false;
        _host = host;
        AdornerLayer.SetAdornedElement(this, _host);
    }

    public bool IsDraggingElements => _isDraggingElements;
    public bool IsActive => _isActive;

    public static readonly StyledProperty<IEnumerable<IEditableControl>?> AdornedElementsProperty = AvaloniaProperty.Register<EditableControlAdorner, IEnumerable<IEditableControl>?>(nameof(EditableControlAdorner.AdornedElements));
    public static readonly StyledProperty<bool> CanRotateProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanRotate), defaultValue: true);
    public static readonly StyledProperty<bool> CanResizeProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanResize), defaultValue: true);
    public static readonly StyledProperty<bool> CanMoveProperty = AvaloniaProperty.Register<EditableControlAdorner, bool>(nameof(EditableControlAdorner.CanMove), defaultValue: true);

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
        _isActive = true;
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
        _isActive = false;
        AdornerLayer layer = AdornerLayer.GetAdornerLayer(_host.FindAncestorOfType<Canvas>()!)!;
        layer.Children.Remove(this);
        _isDraggingElements = false;
    }

    #region Inherited Members

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    #endregion

    private void _MoveElements(Point currentPosition)
    {
        Vector delta = currentPosition - _previousPosition;

        double newLeft = Canvas.GetLeft(_host!) + delta.X;
        double newTop = Canvas.GetTop(_host!) + delta.Y;
        Canvas.SetLeft(_host!, newLeft);
        Canvas.SetTop(_host!, newTop);
        _previousPosition = new Point(newLeft, newTop);

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
        if(!_isActive)
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

        _previousPosition = new Point(adornersMinPositionX, adornersMinPositionY);
    }
}
