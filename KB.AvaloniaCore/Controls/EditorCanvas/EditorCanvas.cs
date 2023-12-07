﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using KB.AvaloniaCore.Controls.UserActions;
using KB.SharpCore.DesignPatterns.UserAction;
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

        _childrenCanvas = new Canvas();
        _editionCanvas = new Canvas();

        _editionCanvas.Children.Add(adornerPlaceholderControl);
        _editionCanvas.Children.Add(_multiSelectBox);


        LogicalChildren.Add(_childrenCanvas);
        LogicalChildren.Add(_editionCanvas);

        VisualChildren.Add(_childrenCanvas);
        VisualChildren.Add(_editionCanvas);

        Children.CollectionChanged += _OnChildrenChanged;

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

        base.OnPointerPressed(e);
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

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if(e.Key == Key.Delete)
        {
            _UpdateSelectedItems(Enumerable.Empty<IEditableControl>());
            SelectedItems.Clear();
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

    private void _UpdateSelectedItems(IEnumerable<IEditableControl> newSelectedItems)
    {
        SelectEditableControlUserAction selectEditableControlUserAction = new SelectEditableControlUserAction(this, SelectedItems, newSelectedItems);
        selectEditableControlUserAction.Do();
        _userActionInvoker.AddUserAction(selectEditableControlUserAction);
    }
}
