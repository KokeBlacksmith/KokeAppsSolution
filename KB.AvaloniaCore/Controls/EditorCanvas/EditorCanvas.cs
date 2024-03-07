using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using KB.AvaloniaCore.Controls.UserActions;
using KB.SharpCore.DesignPatterns.UserAction;
using KB.SharpCore.Events;
using KB.SharpCore.Synchronization;
using KB.SharpCore.Utils;
using System.Collections.Specialized;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Canvas to drag and move elements around.
/// Elemes must implement <see cref="IEditableControl"/>.
/// </summary>
public class EditorCanvas : Control
{
    private enum EStateMachine
    {
        None,
        MultiSelecting,
        Adorner,
    }

    private EStateMachine _stateMachine;

    private bool _isMousePressed;

    private readonly EditableControlAdorner _selectionAdorner;

    private readonly ScrollViewer _scrollViewer;

    /// <summary>
    /// Canvas that will store the IEditbleControl elements.
    /// </summary>
    private readonly Canvas _childrenCanvas;

    /// <summary>
    /// Canvas that will store edition adorners and selection box.
    /// </summary>
    private readonly Canvas _editionCanvas;

    /// <summary>
    /// Multi select box. To select multiple editable controls at once.
    /// </summary>
    private readonly EditorMultiSelectBox _multiSelectBox;

    /// <summary>
    /// Handle Do / Undo actions.
    /// </summary>
    private readonly UserActionInvoker _userActionInvoker;

    /// <summary>
    /// RAII operation to avoid infinite loop when updating selected items.
    /// </summary>
    private BooleanRAIIOperation _selfUpdatingSelectedItemsRAII;

    static EditorCanvas()
    {
        AffectsRender<Panel>(BackgroundProperty);
        EditorCanvas.SelectedItemsProperty.Changed.AddClassHandler<EditorCanvas>((s, e) => s._OnSelectedItemsPropertyChanged(e));
    }

    public EditorCanvas()
    {
        _isMousePressed = false;
        _stateMachine = EStateMachine.None;
        _multiSelectBox = new EditorMultiSelectBox();
        Control adornerPlaceholderControl = new Panel();
        _selectionAdorner = new EditableControlAdorner(adornerPlaceholderControl);
        _selectionAdorner.AdornedElements = SelectedItems;
        _userActionInvoker = new UserActionInvoker();
        _selfUpdatingSelectedItemsRAII = new BooleanRAIIOperation();

        _childrenCanvas = new Canvas();
        _editionCanvas = new Canvas();

        _editionCanvas.Children.Add(adornerPlaceholderControl);
        _editionCanvas.Children.Add(_multiSelectBox);

        _scrollViewer = new ScrollViewer() 
        { 
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        };

        Grid gridCanvasContainer = new Grid();
        gridCanvasContainer.Children.Add(_childrenCanvas);
        gridCanvasContainer.Children.Add(_editionCanvas);
        _scrollViewer.Content = gridCanvasContainer;

        LogicalChildren.Add(_scrollViewer);
        VisualChildren.Add(_scrollViewer);

        Children.CollectionChanged += _OnChildrenChanged;
        _selectionAdorner.OnAdornedElementsMoveFinished += _OnEditableControlsFinishedMoving;
        _selectionAdorner.OnAdornedElementsScaleFinished += _OnEditableControlsFinishedMoving;

        // To receive key events must be focusable
        Focusable = true;
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

        _isMousePressed = true;
        IEditableControl? editableControl = (e.Source as Control)!.FindAncestorOfType<IEditableControl>();
        bool isAdornerClicked = _selectionAdorner.IsPointOver(e.GetPosition(this));

        using (_selfUpdatingSelectedItemsRAII.Execute())
        {
            if(editableControl == null && !isAdornerClicked)
            {
                //User clicked outside any control
                _UpdateSelectedItems(Enumerable.Empty<IEditableControl>());
                _stateMachine = EStateMachine.None;
            }
            else
            {
                if(e.ClickCount == 1)
                {
                    if(isAdornerClicked)
                    {
                        // If pointer pressed started on this canvas, it won't be raised from the child control
                        // So we have to raise it manually
                        _selectionAdorner.OnCanvasPointerPressed(this, e);
                    }
                    else if(!editableControl!.IsSelected)
                    {
                        //Clicked on a non selected item. Select it
                        if (!BitWiseHelper.HasFlag(e.KeyModifiers, KeyModifiers.Control))
                        {
                            // Control key not pressed. Clear selection
                            _UpdateSelectedItems(new IEditableControl[] { editableControl });
                        }
                        else
                        {
                            // Control key pressed. Add to selection
                            IEditableControl[] newSelectedItems = new IEditableControl[SelectedItems.Count + 1];
                            SelectedItems.CopyTo(newSelectedItems, 0);
                            newSelectedItems[^1] = editableControl;
                            _UpdateSelectedItems(newSelectedItems);
                        }

                        _selectionAdorner.Activate();
                        _stateMachine = EStateMachine.Adorner;
                        // If pointer pressed started on this canvas, it won't be raised from the child control
                        // So we have to raise it manually
                        _selectionAdorner.OnCanvasPointerPressed(this, e);
                    }
                    else
                    {
                        // Clicked on a selected item. Unselect it
                        IEnumerable<IEditableControl> newSelectedItems = SelectedItems.Where(item => item != editableControl);
                        _UpdateSelectedItems(newSelectedItems);
                        if(!newSelectedItems.Any())
                        {
                            _stateMachine = EStateMachine.None;
                        }
                    }
                }
                else
                {
                    // Double click
                    _UpdateSelectedItems(new IEditableControl[] { editableControl! });
                
                    //TODO;  ¿What happens on double click?
                }
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if(!_isMousePressed)
        {
            return;
        }

        switch (_stateMachine)
        {
            case EStateMachine.None:
                if (!_multiSelectBox.IsActive)
                {
                    _stateMachine = EStateMachine.MultiSelecting;
                    _multiSelectBox.Start(e.GetPosition(this));
                }
                break;
            case EStateMachine.MultiSelecting:
                _multiSelectBox.Update(e.GetPosition(this));
                break;
            case EStateMachine.Adorner:
                _selectionAdorner.OnCanvasPointerMoved(this, e);
                break;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isMousePressed = false;
        using(_selfUpdatingSelectedItemsRAII.Execute())
        {
            if(_stateMachine == EStateMachine.MultiSelecting)
            {
                IEnumerable<IEditableControl> newSelectedItems = _multiSelectBox.GetSelectedItems(Children.OfType<IEditableControl>().Cast<Visual>()).Cast<IEditableControl>();
                _UpdateSelectedItems(newSelectedItems);
                _multiSelectBox.End();
                if(newSelectedItems.Any())
                {
                    _selectionAdorner.Activate();
                    _stateMachine = EStateMachine.Adorner;
                }
            }
            else if(_stateMachine == EStateMachine.Adorner)
            {
                _selectionAdorner.OnCanvasPointerReleased(this, e);
            }
        }
    }

    // Fired when the SelectedItems property changes. (Not when updated. Ej. Add, remove, clear..)
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

    /// <summary>
    /// Fired when the children collection changes. (Add, Remove, Clear, etc..)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        using(_selfUpdatingSelectedItemsRAII.Execute())
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IEditableControl element in e.NewItems!)
                    {
                        _OnEditableControlAddedToChildrenCollection(element);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    if(e.OldItems != null)
                    {
                        foreach (IEditableControl element in e.OldItems!)
                        {
                            _OnEditableControlRemovedFromChildrenCollection(element);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    foreach (IEditableControl element in e.OldItems!)
                    {
                        _OnEditableControlRemovedFromChildrenCollection(element);
                    }
                    foreach (IEditableControl element in e.NewItems!)
                    {
                        _OnEditableControlAddedToChildrenCollection(element);
                    }
                    break;
            }
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        using(_selfUpdatingSelectedItemsRAII.Execute())
        {
            if(e.Key == Key.Delete)
            {
                _UpdateSelectedItems(Enumerable.Empty<IEditableControl>());
            }
            else if(e.Key == Key.Z && BitWiseHelper.HasFlag(e.KeyModifiers, KeyModifiers.Control))
            {
                _userActionInvoker.Undo();
            }
            else if(e.Key == Key.Y && BitWiseHelper.HasFlag(e.KeyModifiers, KeyModifiers.Control))
            {
                _userActionInvoker.Redo();
            }
        }
    }

    private void _UpdateSelectedItems(IEnumerable<IEditableControl> newSelectedItems)
    {
        SelectEditableControlUserAction selectEditableControlUserAction = new SelectEditableControlUserAction(this, SelectedItems, newSelectedItems);
        if(_userActionInvoker.AddUserAction(selectEditableControlUserAction))
        {
            selectEditableControlUserAction.Do();
        }
    }

    private void _OnEditableControlsFinishedMoving(Point[] oldPositions)
    {
        MoveEditableControlUserAction moveEditableControlUserAction = new MoveEditableControlUserAction(SelectedItems, oldPositions, SelectedItems.Select(item => new Point(item.PositionX, item.PositionY)));
        if(_userActionInvoker.AddUserAction(moveEditableControlUserAction))
        {
            moveEditableControlUserAction.Do();
        }
    }

    private void _OnEditableControlsFinishedMoving(double[] oldWidths, double[] oldHeights)
    {
        ScaleEditableControlUserAction scaleEditableControlUserAction = new ScaleEditableControlUserAction(SelectedItems, oldWidths, oldHeights, SelectedItems.Select(item => item.Width), SelectedItems.Select(item => item.Height));
        if(_userActionInvoker.AddUserAction(scaleEditableControlUserAction))
        {
            scaleEditableControlUserAction.Do();
        }
    }

    private void _OnEditableControlAddedToChildrenCollection(IEditableControl editableControl)
    {
        _childrenCanvas.Children.Add(editableControl.Control);
        editableControl.IsSelectedChanged += _OnEditableControlIsSelectedChanged;

        editableControl.PositionXChanged += _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.PositionYChanged += _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.WidthChanged += _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.HeightChanged += _OnEditableControlLayoutAffectedPropertyChanged;
        _UpdateEditableControlPosition(editableControl);
        InvalidateMeasure();
    }

    private void _OnEditableControlRemovedFromChildrenCollection(IEditableControl editableControl)
    {
        _childrenCanvas.Children.Remove(editableControl.Control);
        editableControl.IsSelectedChanged -= _OnEditableControlIsSelectedChanged;

        editableControl.PositionXChanged -= _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.PositionYChanged -= _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.WidthChanged -= _OnEditableControlLayoutAffectedPropertyChanged;
        editableControl.HeightChanged -= _OnEditableControlLayoutAffectedPropertyChanged;
        InvalidateMeasure();
    }

    /// <summary>
    /// When the IsSelected property of an IEditableControl changes, this method will be called.
    /// To handle IsSelectd changes from outside EditorCanvas.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="valueChangedArgs"></param>
    private void _OnEditableControlIsSelectedChanged(object? sender, ValueChangedEventArgs<bool> valueChangedArgs)
    {
        if(!_selfUpdatingSelectedItemsRAII.CanExecute())
        {
            return;
        }

        // At this point, the IsSelected property was updated from an action outside EditorCanvas.
        IEditableControl editableControl = (IEditableControl)sender!;
        if(editableControl.IsSelected)
        {
            _UpdateSelectedItems(SelectedItems.Append(editableControl));
        }
        else
        {
            _UpdateSelectedItems(SelectedItems.Where(item => item.IsSelected));
        }
    }

    private void _OnEditableControlLayoutAffectedPropertyChanged(object? sender, EventArgs args)
    {
        IEditableControl editableControl = (IEditableControl)sender!;
        _UpdateEditableControlPosition(editableControl);
    }

    private void _UpdateEditableControlPosition(IEditableControl editableControl)
    {
        Canvas.SetLeft(editableControl.Control, editableControl.PositionX);
        Canvas.SetTop(editableControl.Control, editableControl.PositionY);

        //if(!_childrenCanvas.IsInitialized || !_selfUpdatingSelectedItemsRAII.CanExecute())
        //{
        //    return;
        //}

        //using (_selfUpdatingSelectedItemsRAII.Execute())
        //{
        //    // Check canvas must be resized to fit the control.
        //    // The control may be dragged outside the canvas. So the canvas must be resized to fit the control.
        //    double newWidth = editableControl.PositionX + editableControl.Width;
        //    double newHeight = editableControl.PositionY + editableControl.Height;

        //    double? leftChange = null;
        //    double? topChange = null;

        //    if(newWidth > this.Bounds.Width)
        //    {
        //        this.Width = newWidth;
        //    }
        //    else if(editableControl.PositionX < 0.0d)
        //    {
        //        leftChange = System.Math.Abs(editableControl.PositionX);
        //        this.Width = this.Bounds.Width + leftChange.Value;
        //    }

        //    if(newHeight > this.Bounds.Height)
        //    {
        //        this.Height = newHeight;
        //    }
        //    else if(editableControl.PositionY < 0.0d)
        //    {
        //        topChange = System.Math.Abs(editableControl.PositionY);
        //        this.Height = this.Bounds.Height + topChange.Value;
        //    }

        //    if(leftChange.HasValue || topChange.HasValue)
        //    {
        //        foreach(IEditableControl child in Children.Except(SelectedItems))
        //        {
        //            if(leftChange.HasValue)
        //            {
        //                Canvas.SetLeft(child.Control, child.PositionX + leftChange.Value);
        //            }

        //            if(topChange.HasValue)
        //            {
        //                Canvas.SetTop(child.Control, child.PositionY + topChange.Value);
        //            }
        //        }
        //    }
        //}
    }
}
