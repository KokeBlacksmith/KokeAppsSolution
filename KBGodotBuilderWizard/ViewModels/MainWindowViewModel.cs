using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using JetBrains.Annotations;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private string? _selectedDownload;
    private GodotVersionViewModel? _selectedVersion;
    private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
    private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();
    private readonly BusyOperation _updateVersionsBusyOperation;

    public MainWindowViewModel()
    {
        RefreshAvailableVersionsCommand = ReactiveCommand.Create(_FetchVersions);
        _updateVersionsBusyOperation = new BusyOperation(this, nameof(MainWindowViewModel.IsUpdatingVersions));

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

    public string? SelectedDownload
    {
        get { return _selectedDownload; }
        set { this.RaiseAndSetIfChanged(ref _selectedDownload, value); }
    }
    
    public bool IsUpdatingVersions
    {
        get { return _updateVersionsBusyOperation.IsBusy; }
    }

    public ICommand RefreshAvailableVersionsCommand { get; }

    /// <summary>
    ///     Retrieves the available number versions
    /// </summary>
    private async void _FetchVersions()
    {
        using (IDisposable _ = StartBusyOperation())
        {
            await Task.Run(async () =>
            {
                IEnumerable<GodotVersionViewModel> godotVersions = (await GodotVersionFetcher.FetchVersions()).Select(strVersion => new GodotVersionViewModel(strVersion));
                VersionsList = new AvaloniaList<GodotVersionViewModel>(godotVersions);
            });
        }
    }

    private async void _FetchVersionDownloads([NotNull] GodotVersionViewModel version, string extendPath = "/")
    {
        using (IDisposable _ = StartBusyOperation())
        {
            await Task.Run(async () =>
            {
                string urlParentFolderName = Path.GetDirectoryName(extendPath)!;
                // Fetch from the web
                foreach (GodotVersionFetcher.GodotInstallData download in await GodotVersionFetcher.FetchVersionDownloads(version.Version + extendPath))
                {
                    if (!Path.HasExtension(download.FileName))
                    {
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
}