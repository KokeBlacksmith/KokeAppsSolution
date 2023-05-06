using System.Windows.Input;
using Avalonia.Interactivity;
using KBAvaloniaCore.Miscellaneous;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class ConfigurationWindowViewModel : BaseViewModel
{
    private string _installsPath = "Placeholder install path";

    public ConfigurationWindowViewModel()
    {
        SaveCommand = ReactiveCommand.Create(_SaveCommandExecute);
        CancelCommand = ReactiveCommand.Create(_CancelCommandExecute);
    }
    
    public string InstallsPath
    {
        get { return _installsPath; }
        set { this.RaiseAndSetIfChanged(ref _installsPath, value); }
    }
    
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    
    private void _SaveCommandExecute()
    {
        
    }
    
    private void _CancelCommandExecute()
    {
           
    }
}