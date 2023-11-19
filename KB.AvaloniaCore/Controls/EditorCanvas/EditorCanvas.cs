using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using KB.SharpCore.Events;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Canvas to drag and move elements around.
/// Elemes must implement <see cref="IEditableControl"/>.
/// </summary>
public class EditorCanvas : Canvas
{
    private bool _isDraggingElement;
    private Point _previousMousePosition;

    private readonly EditorMultiSelectBox _multiSelectBox;

    static EditorCanvas()
    {
        EditorCanvas.SelectedItemsProperty.Changed.AddClassHandler<EditorCanvas>((s, e) => s.m_OnSelectedItemsPropertyChanged(e));
    }

    public EditorCanvas()
    {
        _isDraggingElement = false;
        _previousMousePosition = default(Point);
        _multiSelectBox = new EditorMultiSelectBox();
    }

    #region StyledProperties

    public static readonly StyledProperty<AvaloniaList<IEditableControl>> SelectedItemsProperty 
        = AvaloniaProperty.Register<EditorCanvas, AvaloniaList<IEditableControl>>(nameof(EditorCanvas.SelectedItems), 
                                        defaultValue: new AvaloniaList<IEditableControl>(), defaultBindingMode: BindingMode.OneWayToSource);


    public AvaloniaList<IEditableControl> SelectedItems
    {
        get { return GetValue(SelectedItemsProperty); }
        private set { SetValue(SelectedItemsProperty, value); }
    }
    
    #endregion

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if(e.ClickCount == 1 && properties.IsLeftButtonPressed)
        {
            IEditableControl? editableControl = (e.Source as Control)!.FindAncestorOfType<IEditableControl>();
            if(editableControl != null)
            {
                editableControl.IsSelected = !editableControl.IsSelected;
                SelectedItems.Add(editableControl);
                editableControl.IsSelected = true;
                _isDraggingElement = true;
                _previousMousePosition = e.GetPosition(this);
                e.Handled = true;
            }
            else
            {
                foreach(IEditableControl editable in SelectedItems!)
                {
                    editable.IsSelected = false;
                }

                SelectedItems.Clear();
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

        foreach(IEditableControl editable in SelectedItems!)
        {
            editable.PositionX += delta.X;
            // Mouse position is Top Left corner. But canvas may return NaN on GetTop of the element so we negate the delta on Y.
            // Node is positioned by Bottom Left corner.
            editable.PositionY -= delta.Y;
        }

        base.OnPointerMoved(e);
    }

    private void m_OnSelectedItemsPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(e.OldValue is AvaloniaList<IEditableControl> oldList)
        {
            foreach(IEditableControl element in oldList)
            {
                element.IsSelected = false;
            }

            //oldList.CollectionChanged -= _OnSelectedItemsCollectionChanged;
        }

        if(e.NewValue is AvaloniaList<IEditableControl> newList)
        {
            foreach(IEditableControl element in newList)
            {
                element.IsSelected = true;
            }

            //newList.CollectionChanged += _OnSelectedItemsCollectionChanged;
        }
    }
}
