using System;
using System.Security.Principal;
using System.Windows.Input;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotInstallViewModel : BaseViewModel
{
    private string _version;
    private string _name;
    private EOperatingSystem _operatingSystem;
    private EProcessor _processorBits;
    private string _installPath;
    private string _urlParentFolderName;
    private bool _isMonoVersion;

    public GodotInstallViewModel(string version, string name, string urlParentFolderName)
    {
        this.Version = version ?? throw new ArgumentNullException(nameof(version));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.UrlParentFolderName = urlParentFolderName ?? "stable";
        _Initialize();
        
        DownloadVersionCommand = ReactiveCommand.Create(_DownloadVersionCommandExecute);
    }

    public string Version
    {
        get { return _version; }
        set { this.RaiseAndSetIfChanged(ref _version, value); }
    }

    public string Name
    {
        get { return _name; }
        set { this.RaiseAndSetIfChanged(ref _name, value); }
    }

    public EOperatingSystem OperatingSystem
    {
        get { return _operatingSystem; }
        set { this.RaiseAndSetIfChanged(ref _operatingSystem, value); }
    }

    public EProcessor Processor
    {
        get { return _processorBits; }
        set { this.RaiseAndSetIfChanged(ref _processorBits, value); }
    }

    public string InstallPath
    {
        get { return _installPath; }
        set { this.RaiseAndSetIfChanged(ref _installPath, value); }
    }
    
    public string UrlParentFolderName
    {
        get { return _urlParentFolderName; }
        set { this.RaiseAndSetIfChanged(ref _urlParentFolderName, value); }
    }

    public bool IsMonoVersion
    {
        get { return _isMonoVersion; }
        set { this.RaiseAndSetIfChanged(ref _isMonoVersion, value); }
    }

    public ICommand DownloadVersionCommand { get; }

    private void _Initialize()
    {
        IsMonoVersion = Name.Contains("mono");

        if (Name.Contains("64"))
        {
            Processor = EProcessor.X64;
        }
        else if (Name.Contains("32") || Name.Contains("86"))
        {
            Processor = EProcessor.X86;
        }
        else
        {
            Processor = EProcessor.ARM;
        }
        
        if (Name.Contains("win"))
        {
            OperatingSystem = EOperatingSystem.Windows;    
        }
        else if (Name.Contains("osx") || Name.Contains("macos"))
        {
            OperatingSystem = EOperatingSystem.MacOS;
        }
        else if (Name.Contains("linux_server"))
        {
            OperatingSystem = EOperatingSystem.LinuxServer;
        }
        else if (Name.Contains("x11") || Name.Contains("linux"))
        {
            OperatingSystem = EOperatingSystem.Linux;
        }
        
        
        // bool isStableVersion = Name.Contains("stable");
    }

    private void _DownloadVersionCommandExecute()
    {
        
    }
}