using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KB.AvaloniaCore.IO;
using KB.AvaloniaCore.MessageBox;
using KB.AvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Enums;
using KBGodotBuilderWizard.Models;
using ReactiveUI;
using Path = KB.AvaloniaCore.IO.Path;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotExecutableViewModel : BaseViewModel, IReactiveModel<GodotExecutable>
{
    private string? _installPath;
    private bool _isMonoVersion;
    private string _fileName;
    private EOperatingSystem _operatingSystem;
    private EProcessor _processorBits;
    private string _urlParentFolderName;
    private string _version;
    private bool _isInstalled;

    public GodotExecutableViewModel(GodotExecutable model)
    {
        this.Model = model;
        this.FromModel(model);
    }

    public string Version
    {
        get { return _version; }
        set { this.RaiseAndSetIfChanged(ref _version, value); }
    }

    public string FileName
    {
        get { return _fileName; }
        set { this.RaiseAndSetIfChanged(ref _fileName, value); }
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

    public string? InstallPath
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

    public bool IsInstalled
    {
        get { return _isInstalled; }
        set { this.RaiseAndSetIfChanged(ref _isInstalled, value); }
    }
    
    public async Task<Result> DownloadAsync(Path installsPath)
    {
        if (installsPath == null)
        {
            return Result.CreateFailure("installsPath is null");
        }

        Result result = await Model.DownloadAsync(installsPath);
        this.IsInstalled = Model.IsInstalled;
        return result;
    }

    public Result Uninstall()
    {
        return Model.Uninstall();
    }

    public void Launch()
    {
        Result result = Model.Launch();
        if (result.IsFailure)
        {
            MessageBoxHelper.ShowResultMessageDialog(result);
        }
    }

    #region IReactiveModel
    
    public GodotExecutable Model { get; }
    public void FromModel(GodotExecutable model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        this.Version = model.Version;
        this.FileName = model.FileName;
        this.OperatingSystem = model.OperatingSystem;
        this.Processor = model.Processor;
        this.InstallPath = model.InstallPath;
        this.UrlParentFolderName = model.UrlParentFolderName;
        this.IsMonoVersion = model.IsMonoVersion;
        this.IsInstalled = model.IsInstalled;
    }

    public void UpdateModel()
    {
        Model.Version = this.Version;
        Model.FileName = this.FileName;
        Model.OperatingSystem = this.OperatingSystem;
        Model.Processor = this.Processor;
        Model.InstallPath = this.InstallPath;
        Model.UrlParentFolderName = this.UrlParentFolderName;
        Model.IsMonoVersion = this.IsMonoVersion;
        Model.IsInstalled = this.IsInstalled;
    }
    
    #endregion
}