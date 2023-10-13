using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace KB.AvaloniaCore.Injection;

public static class ControlExtension
{
    public static T? FindChildOfType<T>(this Control control) 
        where T : Control
    {
        foreach (Visual child in control.GetVisualDescendants())
        {
            if(child is T typedChild)
            {
                return typedChild;
            }
        }

        return null;
    }
    
    public static T GetChildOfType<T>(this Control control) 
        where T : Control
    {
        return control.FindChildOfType<T>() ?? throw new Exception($"Could not find child of type {typeof(T).Name} in the parent control {control}.");
    }
}