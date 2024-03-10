using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using KB.AvaloniaCore.Controls;
using KB.AvaloniaCore.Injection;
using KB.SharpCore.Utils;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reactive;

namespace KB.ConsoleCompanion.MacroEditView;

public partial class MacroEditView : UserControl
{
    public MacroEditView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, _OnDrop);
        AddHandler(DragDrop.DragOverEvent, _OnDragOver);
    }

    private void _OnDragOver(object? sender, DragEventArgs e)
    {
    }

    private void _OnDrop(object? sender, DragEventArgs e)
    {
    }
}
