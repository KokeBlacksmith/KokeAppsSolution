using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private GodotInstallViewModel? _selectedDownload;
    private GodotVersionViewModel? _selectedVersion;
    private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
    private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();
    private readonly BusyOperation _updateVersionsBusyOperation;

    public MainWindowViewModel()
    {
        _updateVersionsBusyOperation = new BusyOperation(this, nameof(MainWindowViewModel.IsUpdatingVersions));
        
        RefreshAvailableVersionsCommand = ReactiveCommand.Create(_FetchVersions);
        
        IObservable<bool> canDownload = this.WhenAnyValue(
            property1: x => x.IsUpdatingVersions,
            property2: y => y.SelectedDownload,
            selector: (isUpdating, selected) => !isUpdating && selected != null);
        DownloadVersionCommand = ReactiveCommand.Create(_DownloadVersionCommandExecute, canDownload);
    }

    public AvaloniaList<GodotVersionViewModel> VersionsList
    {
        get { return _versionsList; }
        set { this.RaiseAndSetIfChanged(ref _versionsList, value); }
    }

    public AvaloniaList<GodotVersionViewModel> SelectedVersionDownloadList
    {
        get { return _selectedVersionDownloadList; }
        set { this.RaiseAndSetIfChanged(ref _selectedVersionDownloadList, value); }
    }

    public GodotVersionViewModel? SelectedVersion
    {
        get { return _selectedVersion; }
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedVersion, value);
            _FetchVersionDownloads(_selectedVersion);
        }
    }

    public GodotInstallViewModel? SelectedDownload
    {
        get { return _selectedDownload; }
        set { this.RaiseAndSetIfChanged(ref _selectedDownload, value); }
    }

    public bool IsUpdatingVersions
    {
        get { return _updateVersionsBusyOperation.IsBusy; }
    }

    public ICommand RefreshAvailableVersionsCommand { get; }
    public ICommand DownloadVersionCommand { get; }

    /// <summary>
    ///     Retrieves the available number versions
    /// </summary>
    private async void _FetchVersions()
    {
        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await Task.Run(async () =>
            {
                IEnumerable<GodotVersionViewModel> godotVersions = (await GodotVersionFetcher.FetchVersions()).Select(strVersion => new GodotVersionViewModel(strVersion));
                VersionsList = new AvaloniaList<GodotVersionViewModel>(godotVersions);
            });
        }
    }

    private async void _FetchVersionDownloads([JetBrains.Annotations.NotNull] GodotVersionViewModel version, string extendPath = "/")
    {
        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await Task.Run(async () =>
            {
                string urlParentFolderName = Path.GetDirectoryName(extendPath)!;
                // Fetch from the web
                foreach (GodotVersionFetcher.GodotInstallData download in await GodotVersionFetcher.FetchVersionDownloads(version.Version + extendPath))
                {
                    if (!Path.HasExtension(download.FileName))
                    {
                        //FileName contains the directories to the file
                        
                        // It is a folder
                        _FetchVersionDownloads(version, $"{extendPath}{download.FileName}");
                    }
                    else
                    {
                        version.AddInstall(download.FileName, urlParentFolderName);
                    }
                }
            });
        }
    }

    [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH")]
    private async void _DownloadVersionCommandExecute()
    {
        using (IDisposable _ = _updateVersionsBusyOperation.StartOperation())
        {
            await Task.Run(async () =>
            {
                ConfigurationFileData configurationFileData = new ConfigurationFileData();
                Result result = configurationFileData.Load();
                
                if (result.IsFailure)
                {
                    //TODO: Show error
                    return;    
                }
                
                string fetchUrl = this.SelectedDownload!.GetPartialUrl();
                KBAvaloniaCore.IO.Path destinationPath = KBAvaloniaCore.IO.Path.Combine(configurationFileData.InstallVersionsPath.FullPath, this.SelectedDownload!.Name, this.SelectedDownload!.Name);
                destinationPath = new KBAvaloniaCore.IO.Path(destinationPath.FullPath.Replace('.', '_'));

                destinationPath.DeleteDirectory(true);
                destinationPath.CreateDirectory();
                
                result = await GodotVersionFetcher.DownloadVersion(destinationPath, fetchUrl);
                
                if (result.IsFailure)
                {
                    //TODO: Show error
                    return;
                }

                string destinationUnzip = destinationPath.TryGetParent(out KBAvaloniaCore.IO.Path parentPath) ? parentPath.FullPath : destinationPath.FullPath;
                ZipFile.ExtractToDirectory(destinationPath.FullPath, destinationUnzip, true);
                File.Delete(destinationPath.FullPath);
            });
        }
    }

    private async void _LaunchVersionCommandExecute()
    {
        
    }
}