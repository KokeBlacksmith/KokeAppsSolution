using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KB.ConsoleCompanionStandalone.Views;

namespace KB.ConsoleCompanionStandalone
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow {
                    DataContext = null,
                };
            }
        
            base.OnFrameworkInitializationCompleted();
        }
    }
}