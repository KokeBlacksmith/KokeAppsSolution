using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Interactivity;
using DynamicData.Binding;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;
using Path = KBAvaloniaCore.IO.Path;

namespace KBGodotBuilderWizard.ViewModels;

public class ConfigurationWindowViewModel : BaseViewModel
{
    private string _installsPath = "bobo Placeholder install path";

    public ConfigurationWindowViewModel()
    {
        IObservable<bool> canSave = this
            .WhenAnyValue(x => x.InstallsPath, (path) => new KBAvaloniaCore.IO.Path(path).Exists());
        
        SaveCommand = ReactiveCommand.Create(_SaveCommandExecute, canSave);
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
        ConfigurationFileData configurationFileData = new ConfigurationFileData();
        configurationFileData.InstallVersionsPath = InstallsPath;
        configurationFileData.Save();
    }
    
    private void _CancelCommandExecute()
    {
        
    }
}