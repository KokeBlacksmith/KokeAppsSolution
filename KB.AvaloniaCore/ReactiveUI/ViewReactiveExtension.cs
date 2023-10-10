using Avalonia;
using Avalonia.Data;
using ReactiveUI;

namespace KB.AvaloniaCore.ReactiveUI;

public static class ViewReactiveExtension
{
    public static bool StyledPropertyHasBinding<T>(this AvaloniaObject self, StyledProperty<T> property)
    {
        // I want to get the binding from a styled property in avalonia to check if it has a binding and the BindingDirection of it
        // I have the reference of the AvaloniaObject and the styled property
        
        
        return false;
    }
    
    public static bool StyledPropertyHasBindingMode<T>(this AvaloniaObject self, StyledProperty<T> property, BindingMode mode)
    {
        // I want to get the binding from a styled property in avalonia to check if it has a binding and the mode of it
        // I have the reference of the AvaloniaObject and the styled property
        

        return true;
    }
}