using Avalonia;
using Avalonia.Media;

namespace KB.AvaloniaCore.Injection;

public static class RenderBackgroundUtils
{
    public static void DrawGrid(DrawingContext context, Rect bounds, IBrush lineBrush, double spacing)
    {
        Pen pen = new Pen(lineBrush, 1);

        for (double x = 0; x < bounds.Width; x += spacing)
        {
            context.DrawLine(pen, new Point(x, 0), new Point(x, bounds.Height));
        }

        for (double y = 0; y < bounds.Height; y += spacing)
        {
            context.DrawLine(pen, new Point(0, y), new Point(bounds.Width, y));
        }
    }
}
