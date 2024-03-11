using Avalonia.Controls;
using Avalonia.Input;

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
