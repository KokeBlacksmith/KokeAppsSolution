using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Events;

namespace KB.AvaloniaCore.Behaviors.Events
{
    public class DoubleTappedEventSubscriber : DoubleTappedEventBehavior
    {
        
        public event EventHandler<RoutedEventArgs>? DoubleTapped;

        protected override void OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            DoubleTapped?.Invoke(sender, e);
        }
    }
}
