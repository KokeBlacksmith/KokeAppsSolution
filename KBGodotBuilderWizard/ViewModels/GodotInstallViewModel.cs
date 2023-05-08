using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;
using Path = KBAvaloniaCore.IO.Path;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotInstallViewModel : BaseViewModel
{
    private string _installPath;
    private bool _isMonoVersion;
    private string _name;
    private EOperatingSystem _operatingSystem;
    private EProcessor _processorBits;
    private string _urlParentFolderName;
    private string _version;

    public GodotInstallViewModel(string version, string name, string? urlParentFolderName)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UrlParentFolderName = urlParentFolderName;
        _Initialize();
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

    public string GetPartialUrl()
    {
        return $"{Version}{UrlParentFolderName}/{Name}.zip".Replace('\\', '/');
    }

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
        else if (Name.Contains("web"))
        {
            OperatingSystem = EOperatingSystem.Web;
        }
    }

    public Task<Result> DownloadAsync()
    {
        return Task.Run(async () =>
        {
            ConfigurationFileData configurationFileData = new ConfigurationFileData();
            Result result = configurationFileData.Load();

            if (result.IsFailure)
            {
                return result;
            }

            Path destinationPath = Path.Combine(configurationFileData.InstallVersionsPath.FullPath, Name, Name);
            destinationPath = new Path(destinationPath.FullPath.Replace('.', '_'));

            destinationPath.DeleteDirectory(true);
            destinationPath.CreateDirectory();

            result = await GodotVersionFetcher.DownloadVersion(destinationPath, GetPartialUrl());

            if (result.IsFailure)
            {
                return result;
            }

            string destinationUnzip = destinationPath.TryGetParent(out Path parentPath) ? parentPath.FullPath : destinationPath.FullPath;
            ZipFile.ExtractToDirectory(destinationPath.FullPath, destinationUnzip, true);
            destinationPath.DeleteFile();
            File.Delete(destinationPath.FullPath);

            return Result.CreateSuccess();
        });
        
    }

    public Result Uninstall()
    {
        return Result.CreateSuccess();
    }
}