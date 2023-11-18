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
    public IEditableControl? SelectedElement { get; private set; }

    public EventHandler<ValueChangedEventArgs<IEditableControl>>? SelectedElementChanged;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if(e.ClickCount == 1)
        {
            IEditableControl? editableControl = (e.Source as Control)!.FindAncestorOfType<IEditableControl>();
            if(editableControl != null)
            {
                editableControl.IsSelected = !editableControl.IsSelected;
                SelectedElement = editableControl;
                _isDraggingElement = true;
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

        var positionPoint = e.GetPosition(this);

        Canvas.SetLeft((Control)SelectedElement!, positionPoint.X);
        Canvas.SetTop((Control)SelectedElement!, positionPoint.Y);

        base.OnPointerMoved(e);
    }
}
