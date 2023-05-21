using System;
using System.Windows.Input;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class ConfigurationWindowViewModel : BaseViewModel
{
    private string _installsPath = "bobo Placeholder install path";

    public ConfigurationWindowViewModel()
    {
        IObservable<bool> canSave = this.WhenAnyValue(x => x.InstallsPath, path => new Path(path).Exists());

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
        ConfigurationData configurationData = new ConfigurationData();
        configurationData.InstallVersionsPath = InstallsPath;
        configurationData.Save();
    }

    private void _CancelCommandExecute() { }
}