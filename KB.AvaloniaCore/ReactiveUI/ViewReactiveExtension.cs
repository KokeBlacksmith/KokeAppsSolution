using Avalonia;
using Avalonia.Data;
using ReactiveUI;

namespace KB.AvaloniaCore.ReactiveUI;

public static class ViewReactiveExtension
{
    public static IDisposable SubscribeToStyledPropertyChanged<TView, T>(this TView self, StyledProperty<T> property, Action<TView, AvaloniaPropertyChangedEventArgs> action)
        where TView : AvaloniaObject
    {
        return property.Changed.AddClassHandler<TView>(action);
    }
}