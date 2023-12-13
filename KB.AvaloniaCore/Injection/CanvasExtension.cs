using Avalonia;
using Avalonia.Controls;

namespace KB.AvaloniaCore.Injection;
public static class CanvasExtension
{
    /// <summary>
    /// Returns true if the given point is over the given child control.
    /// <para/>
    /// Avalonia has the property <see cref="InputElement.IsPointerOver"/> but it only works if the upper control is this one.
    /// So if you have a canvas with a child control and you want to know if the mouse is over the child control, you have to use this method.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="point"></param>
    /// <param name="child"></param>
    /// <returns>Returns true if the given point is over the given child control.</returns>
    public static bool IsPointOverChild(this Canvas self, Point point, Control child)
    {
        return IsPointOverCanvasChild(point, child);
    }

    /// <summary>
    /// Returns true if the given point is over the given child control.
    /// It relative to the child control's parent canvas.
    /// <para/>
    /// Avalonia has the property <see cref="InputElement.IsPointerOver"/> but it only works if the upper control is this one.
    /// So if you have a canvas with a child control and you want to know if the mouse is over the child control, you have to use this method.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="point"></param>
    /// <param name="child"></param>
    /// <returns>Returns true if the given point is over the given child control.</returns>
    public static bool IsPointOverCanvasChild(Point point, Control child)
    {
        double left = Canvas.GetLeft(child!);
        double top = Canvas.GetTop(child!);
        double right = left + child.Width;
        double bottom = top + child.Height;
        return left <= point.X && right >= point.X && top <= point.Y && bottom >= point.Y;
    }

    public static Point GetControlCenter(this Canvas self, Control control)
    {
        return GetCanvasControlCenter(control);
    }

    public static Point GetCanvasControlCenter(Control control)
    {
        double left = Canvas.GetLeft(control);
        double top = Canvas.GetTop(control);
        return new Point(left + control.GetHalfWidth(), top + control.GetHalfHeight());
    }

    public static Point GetControlLeftTop(Control control)
    {
        double left = Canvas.GetLeft(control);
        double top = Canvas.GetTop(control);
        return new Point(left, top);
    }
}
