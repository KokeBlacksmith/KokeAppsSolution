using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using KB.SharpCore.Utils;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Canvas to drag and move elements around.
/// Elemes must implement <see cref="IEditableControl"/>.
/// </summary>
public class EditorCanvas : Control
{
    private readonly EditableControlAdorner _selectionAdorner;

    /// <summary>
    /// Canvas that will store the IEditbleControl elements.
    /// </summary>
    private readonly Canvas _childrenCanvas;

    /// <summary>
    /// Canvas that will store edition adorners and selection box.
    /// </summary>
    private readonly Canvas _editionCanvas;

    private readonly EditorMultiSelectBox _multiSelectBox;

    static EditorCanvas()
    {
        AffectsRender<Panel>(BackgroundProperty);
        EditorCanvas.SelectedItemsProperty.Changed.AddClassHandler<EditorCanvas>((s, e) => s._OnSelectedItemsPropertyChanged(e));
    }

    public EditorCanvas()
    {
        _multiSelectBox = new EditorMultiSelectBox();
        Control adornerPlaceholderControl = new Panel();
        _selectionAdorner = new EditableControlAdorner(adornerPlaceholderControl);
        _selectionAdorner.AdornedElements = SelectedItems;

        _childrenCanvas = new Canvas();
        _editionCanvas = new Canvas();

        _editionCanvas.Children.Add(adornerPlaceholderControl);
        _editionCanvas.Children.Add(_multiSelectBox);


        LogicalChildren.Add(_childrenCanvas);
        LogicalChildren.Add(_editionCanvas);

        VisualChildren.Add(_childrenCanvas);
        VisualChildren.Add(_editionCanvas);

        Children.CollectionChanged += _OnChildrenChanged;
    }

    /// <summary>
    /// Gets the children of the <see cref="Panel"/>.
    /// </summary>
    [Content]
    public AvaloniaList<IEditableControl> Children { get; } = new AvaloniaList<IEditableControl>();

    #region StyledProperties

    public static readonly StyledProperty<AvaloniaList<IEditableControl>> SelectedItemsProperty 
        = AvaloniaProperty.Register<EditorCanvas, AvaloniaList<IEditableControl>>(nameof(EditorCanvas.SelectedItems), 
                                        defaultValue: new AvaloniaList<IEditableControl>(), defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// Defines the <see cref="Background"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty = Border.BackgroundProperty.AddOwner<EditorCanvas>();

    public AvaloniaList<IEditableControl> SelectedItems
    {
        get { return GetValue(SelectedItemsProperty); }
        private set { SetValue(SelectedItemsProperty, value); }
    }

    /// <summary>
    /// Gets or Sets Editor Canvas background brush.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    #endregion

    public bool IsEdittingControls
    {
        get { return SelectedItems.Count > 0; }
    }

    /// <summary>
    /// Renders the visual to a <see cref="DrawingContext"/>.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public sealed override void Render(DrawingContext context)
    {
        var background = Background;
        if (background != null)
        {
            var renderSize = Bounds.Size;
            context.FillRectangle(background, new Rect(renderSize));
        }

        base.Render(context);
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
                    if (!_selectionAdorner.IsActive)
                    {
                        _selectionAdorner.Activate();
                    }
                }
            }
            else
            {
                _RemoveEditAdorner();
            }
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
                if (!_selectionAdorner.IsActive)
                {
                    _selectionAdorner.Activate();
                }
            }
            else if(_selectionAdorner.IsActive)
            {
                _selectionAdorner.Deactivate();
            }

            _multiSelectBox.End();
        }
    }

    private void _RemoveEditAdorner()
    {
        _selectionAdorner.Deactivate();

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

    private void _OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IEditableControl element in e.NewItems!)
            {
                _childrenCanvas.Children.Add((Control)element);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IEditableControl element in e.OldItems!)
            {
                _childrenCanvas.Children.Remove((Control)element);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            _childrenCanvas.Children.Clear();
        }
        else if (e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Move)
        {
            foreach (IEditableControl element in e.OldItems!)
            {
                _childrenCanvas.Children.Remove((Control)element);
            }

            _childrenCanvas.Children.AddRange(e.NewItems!.OfType<IEditableControl>().Cast<Control>());
        }
    }
}
