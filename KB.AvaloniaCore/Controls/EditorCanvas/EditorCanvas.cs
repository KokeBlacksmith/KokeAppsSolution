using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using KB.SharpCore.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Canvas to drag and move elements around.
/// Elemes must implement <see cref="IEditableControl"/>.
/// </summary>
public class EditorCanvas : Canvas
{
    private bool _isDraggingElement;
    private Point _previousMousePosition;

    public IEditableControl? SelectedElement { get; private set; }

    public EventHandler<ValueChangedEventArgs<IEditableControl>>? SelectedElementChanged;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if(e.ClickCount == 1 && properties.IsLeftButtonPressed)
        {
            IEditableControl? editableControl = (e.Source as Control)!.FindAncestorOfType<IEditableControl>();
            if(editableControl != null)
            {
                editableControl.IsSelected = !editableControl.IsSelected;
                SelectedElement = editableControl;
                _isDraggingElement = true;
                _previousMousePosition = e.GetPosition(this);
                e.Handled = true;
            }
            else
            {
                SelectedElement = null;
                _isDraggingElement = false;
            }
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isDraggingElement = false;
        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!_isDraggingElement)
        {
            return;
        }

        Point positionPoint = e.GetPosition(this);
        Point delta = positionPoint - _previousMousePosition;
        _previousMousePosition = positionPoint;

        SelectedElement!.PositionX += delta.X;
        // Mouse position is Top Left corner. But canvas may return NaN on GetTop of the element so we negate the delta on Y.
        // Node is positioned by Bottom Left corner.
        SelectedElement!.PositionY -= delta.Y;

        base.OnPointerMoved(e);
    }
}
