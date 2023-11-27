using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Draggable;
using System;
using System.Collections.Generic;
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
    private Control? _host;
    private bool _isActive;

    static EditableControlAdorner()
    {
        EditableControlAdorner.AdornedElementsProperty.Changed.AddClassHandler<EditableControlAdorner>((s, e) => s._OnAdornedElementsPropertyChanged(e));
    }


    public EditableControlAdorner()
    {
        IsHitTestVisible = false;
        _isDraggingElements = false;
        _isActive = false;
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
    }

    public void Activate(AdornerLayer layer, Control host)
    {
        _isActive = true;
        _host = host;
        AdornerLayer.SetAdornedElement(this, _host);

        layer.Children.Add(this);

        if (AdornedElements == null)
        {
            return;
        }

        // Set the size of the host the same as the size of the adorned elements
        _MeasureHost();

        // Calculate the center of the AdornedElements
        // Then reposition the host so the center of the host is the same as the center of the AdornedElements
        double adornersMaxPositionX = Double.MinValue;
        double adornersMaxPositionY = Double.MinValue;

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
        foreach (IEditableControl editableControl in AdornedElements)
        {
            adornersMaxPositionX = System.Math.Max(adornersMaxPositionX, editableControl.PositionX);
            adornersMaxPositionY = System.Math.Max(adornersMaxPositionY, editableControl.PositionY);
        }
        
        double hostDesiredPositionX = adornersMaxPositionX;
        double hostDesiredPositionY = adornersMaxPositionY;
        Canvas.SetLeft(_host, hostDesiredPositionX);
        Canvas.SetTop(_host, hostDesiredPositionY);

        _previousPosition = new Point(hostDesiredPositionX, hostDesiredPositionY);
        _isDraggingElements = true;
    }

    public void Deactivate(AdornerLayer layer)
    {
        _isActive = false;
        layer.Children.Remove(this);
        _isDraggingElements = false;

        _host = null;
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
        //Canvas.SetLeft(_host!, Canvas.GetLeft(_host!) + delta.X);
        //Canvas.SetBottom(_host!, Canvas.GetBottom(_host!) - delta.Y);

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
        if (AdornedElements != null)
        {
            // Calculate the size of the adorner based on the adorned element. It can have multiple elements so it has to take into account all of them.
            foreach (IEditableControl editableControl in AdornedElements)
            {
                size = new Size(System.Math.Max(size.Width, editableControl.Width), System.Math.Max(size.Height, editableControl.Height));
            }
        }

        _host!.Width = size.Width;
        _host!.Height = size.Height;
    }
}
