using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using KB.AvaloniaCore.Controls;
using KB.AvaloniaCore.Controls.GraphEditor;
using KB.AvaloniaCore.Injection;

using System;
using System.Collections.Specialized;
using System.Reactive;

namespace KB.ConsoleCompanion.MacroEditView;

public partial class MacroEditView : UserControl
{
    static MacroEditView()
    {

        //GraphCanvas.SelectedItemsProperty.Changed.Subscribe(_OnSelectedNodesChanged);
    }

    public MacroEditView()
    {
        InitializeComponent();
        AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(
            args => 
            {
                _OnSelectedNodesChanged((AvaloniaPropertyChangedEventArgs<AvaloniaList<IEditableControl>>)args);
            }
        );

        _graphCanvas.GetPropertyChangedObservable(GraphCanvas.SelectedItemsProperty).Subscribe(observer);
        _InternalOnSelectedNodesChanged(_graphCanvas.SelectedItems);
    }

    private void _OnSelectedNodesChanged(AvaloniaPropertyChangedEventArgs<AvaloniaList<IEditableControl>> args)
    {
        _InternalOnSelectedNodesChanged(args.NewValue.GetValueOrDefault());
        _UpdatePropertyGridItems();
    }

    private void _InternalOnSelectedNodesChanged(AvaloniaList<IEditableControl>? newValue)
    {
        if(_graphCanvas.SelectedItems != null)
        {
            _graphCanvas.SelectedItems.CollectionChanged -= _OnSelectedItemsCollectionChanged;
        }

        if(newValue != null)
        {
            newValue.CollectionChanged += _OnSelectedItemsCollectionChanged;
        }

        _UpdatePropertyGridItems();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        RenderBackgroundUtils.DrawGrid(context, Bounds, Brushes.LightBlue, 30);
    }

    private void _OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _UpdatePropertyGridItems();
    }   

    private void _UpdatePropertyGridItems()
    {
        _propertyGrid.DataContext = _graphCanvas.SelectedItems.ToArray();
    }
    
}
