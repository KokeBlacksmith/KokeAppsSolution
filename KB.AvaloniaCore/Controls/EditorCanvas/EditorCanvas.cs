using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Draggable;
using KB.SharpCore.Utils;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Canvas to drag and move elements around.
/// Elemes must implement <see cref="IEditableControl"/>.
/// </summary>
public class EditorCanvas : Canvas
{
    private readonly EditableControlAdorner _selectionAdorner;
    /// <summary>
    /// Placeholder control to host the <see cref="EditableControlAdorner"/>.
    /// The adorner may have to be over several controls and this one will be used to host it.
    /// </summary>
    private readonly Panel _adornerPlaceholderControl;


    private readonly EditorMultiSelectBox _multiSelectBox;

    static EditorCanvas()
    {
        EditorCanvas.SelectedItemsProperty.Changed.AddClassHandler<EditorCanvas>((s, e) => s._OnSelectedItemsPropertyChanged(e));
    }

    public EditorCanvas()
    {
        _multiSelectBox = new EditorMultiSelectBox();
        _selectionAdorner = new EditableControlAdorner();
        _adornerPlaceholderControl = new Panel();
        _selectionAdorner.AdornedElements = SelectedItems;
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

    public bool IsEdittingControls
    {
        get { return SelectedItems.Count > 0; }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if(e.Handled || !properties.IsLeftButtonPressed)
        {
            return;
        }

        IEditableControl? editableControl = (e.Source as Control)!.FindAncestorOfType<IEditableControl>();
        if(e.ClickCount == 1)
        {
            if(editableControl == null)
            {
                if(IsEdittingControls)
                {
                    // End edit
                    _RemoveEditAdorner();
                }

                // Selection box, clicked outside any control
                _AddInternal(_multiSelectBox);
                _multiSelectBox.Start(e.GetPosition(this));
            }
            else if(editableControl != null)
            {
                bool isNewControl = !SelectedItems.Contains(editableControl);
                bool controlKeyIsPressed = BitWiseHelper.HasFlag(e.KeyModifiers, KeyModifiers.Control);
                bool controlAddedToSelection = false;

                if(IsEdittingControls)
                {
                    if(controlKeyIsPressed && isNewControl)
                    {
                        _AddEditableControlToSelection(editableControl);
                        controlAddedToSelection = true;
                    }
                    else if (isNewControl)
                    {
                        _RemoveEditAdorner();
                        _AddEditableControlToSelection(editableControl);
                        controlAddedToSelection = true;
                    }
                    else if (!controlKeyIsPressed && !isNewControl)
                    {
                        _RemoveEditAdorner();
                        controlAddedToSelection = false;
                    }
                }
                else
                {
                    _AddEditableControlToSelection(editableControl);
                    controlAddedToSelection = true;
                }

                if(controlAddedToSelection)
                {
                    AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null && !_selectionAdorner.IsActive)
                    {
                        _AddInternal(_adornerPlaceholderControl);
                        _selectionAdorner.Activate(adornerLayer, _adornerPlaceholderControl);
                    }
                }
            }
            else
            {
                _RemoveEditAdorner();
            }

            e.Handled = true;
        }
        else
        {
            _RemoveEditAdorner();
        }

        // If pointer pressed started on this canvas, it won't be raised from the child control
        // So we have to raise it manually
        _selectionAdorner.OnCanvasPointerPressed(this, e);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        // If pointer pressed started on this canvas, it won't be raised from the child control
        // So we have to raise it manually
        if(_selectionAdorner.IsActive)
        {
            _selectionAdorner.OnCanvasPointerMoved(this, e);
        }

        if(_multiSelectBox.IsVisible)
        {
            _multiSelectBox.Update(e.GetPosition(this));
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (_selectionAdorner.IsActive)
        {
            _selectionAdorner.OnCanvasPointerReleased(this, e);
        }

        if(_multiSelectBox.IsVisible)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(_multiSelectBox.GetSelectedItems(Children.OfType<IEditableControl>().Cast<Visual>()).Cast<IEditableControl>());

            if(SelectedItems.Count > 0)
            {
                AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null && !_selectionAdorner.IsActive)
                {
                    _AddInternal(_adornerPlaceholderControl);
                    _selectionAdorner.Activate(adornerLayer, _adornerPlaceholderControl);
                }
            }
            else if(_selectionAdorner.IsActive)
            {
                AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    _selectionAdorner.Deactivate(adornerLayer);
                    _RemoveInternal(_adornerPlaceholderControl);
                }
            }

            _multiSelectBox.End();
        }
    }

    private void _RemoveEditAdorner()
    {
        AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer != null)
        {
           _selectionAdorner.Deactivate(adornerLayer);
            _RemoveInternal(_adornerPlaceholderControl);
        }

        foreach (IEditableControl editable in SelectedItems!)
        {
            editable.IsSelected = false;
        }

        SelectedItems.Clear();
    }

    private void _AddEditableControlToSelection(IEditableControl control)
    {
        if(!SelectedItems.Contains(control))
        {
            control.IsSelected = !control.IsSelected;
            SelectedItems.Add(control);
            control.IsSelected = true;
        }
    }

    private void _OnSelectedItemsPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if(e.OldValue is AvaloniaList<IEditableControl> oldList)
        {
            foreach(IEditableControl element in oldList)
            {
                element.IsSelected = false;
            }
        }

        if(e.NewValue is AvaloniaList<IEditableControl> newList)
        {
            foreach(IEditableControl element in newList)
            {
                element.IsSelected = true;
            }
        }

        _selectionAdorner.AdornedElements = SelectedItems;
    }

    private void _AddInternal(Control control)
    {
        //LogicalChildren.Add(control);
        //VisualChildren.Add(control);
        //InvalidateMeasure();

        //TODO: Find a way so that the adorner is not added to the visual tree using Children collection, otherswise it could be removed by the user or client of the library
        if(!Children.Contains(control))
        {
            Children.Add(control);
        }
    }

    private void _RemoveInternal(Control control)
    {
        //LogicalChildren.Remove(control);
        //VisualChildren.Remove(control);
        //InvalidateMeasure();
        this.Children.Remove(control);
    }
}
