using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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

        DrawGrid(context);
    }

    private void DrawGrid(DrawingContext context)
    {
        Pen pen = new Pen(Brushes.LightGray, 1);
        double spacing = 25;

        for (double x = 0; x < Bounds.Width; x += spacing)
        {
            context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
        }

        for (double y = 0; y < Bounds.Height; y += spacing)
        {
            context.DrawLine(pen, new Point(0, y), new Point(Bounds.Width, y));
        }
    }
}
