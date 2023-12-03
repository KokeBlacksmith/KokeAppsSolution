using Avalonia;
using Avalonia.Controls;

namespace KB.AvaloniaCore.Injection;
public static class CanvasExtension
{
    public static bool IsPointOverChild(this Canvas self, Point point, Control child)
    {
        return IsPointOverCanvasChild(point, child);
    }

    public static bool IsPointOverCanvasChild(Point point, Control child)
    {
        double left = Canvas.GetLeft(child!);
        double top = Canvas.GetTop(child!);
        double right = left + child.Width;
        double bottom = top + child.Height;
        return left <= point.X && right >= point.X && top <= point.Y && bottom >= point.Y;
    }
}
