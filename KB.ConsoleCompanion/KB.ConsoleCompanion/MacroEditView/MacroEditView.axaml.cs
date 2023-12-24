using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using KB.AvaloniaCore.Injection;

namespace KB.ConsoleCompanion.MacroEditView;

public partial class MacroEditView : UserControl
{
    public MacroEditView()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        RenderBackgroundUtils.DrawGrid(context, Bounds, Brushes.LightBlue, 30);
    }
}
