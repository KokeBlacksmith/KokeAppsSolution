using KB.AvaloniaCore.ReactiveUI;

namespace KB.ConsoleCompanionStandalone.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public MainWindowViewModel()
    {
    }
    
    public string Greeting => "Welcome to Avalonia Console companion!";
}

