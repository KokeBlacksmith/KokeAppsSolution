using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using KBAvaloniaCore.Miscellaneous;
using KBAvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotVersionHubViewModel : BaseViewModel
{
    private readonly BusyOperation _updateVersionsBusyOperation;
    private GodotExecutableViewModel? _selectedDownload;
    private GodotVersionViewModel? _selectedVersion;
    private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
    private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();

    public GodotVersionHubViewModel()
    {
        _updateVersionsBusyOperation = new BusyOperation(this, nameof(GodotVersionHubViewModel.IsUpdatingVersions));

        RefreshAvailableVersionsCommand = ReactiveCommand.Create(_FetchVersions);

        IObservable<bool> canDownload = this.WhenAnyValue(x => x.IsUpdatingVersions, y => y.SelectedDownload, (isUpdating, selected) => !isUpdating && selected != null);
        DownloadVersionCommand = ReactiveCommand.Create(_DownloadVersionCommandExecute, canDownload);
        IObservable<bool> canInstall = this.WhenAnyValue(x => x.IsUpdatingVersions, y => y.SelectedDownload, (isUpdating, selected) => !isUpdating && selected is { IsInstalled: true });
        LaunchVersionCommand = ReactiveCommand.Create(_LaunchVersionCommandExecute, canInstall);
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

    public GodotExecutableViewModel? SelectedDownload
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
    public ICommand LaunchVersionCommand { get; }

    /// <summary>
    ///     Retrieves the available number versions
    /// </summary>
    private async void _FetchVersions()
    {
        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await Task.Run(async () =>
            {
                IEnumerable<GodotVersionViewModel> godotVersions = (await GodotVersionFetcher.FetchVersions()).Select(version => new GodotVersionViewModel(version));
                VersionsList = new AvaloniaList<GodotVersionViewModel>(godotVersions);
            });
        }
    }

    private async void _FetchVersionDownloads(GodotVersionViewModel version)
    {
        if (version == null)
            throw new ArgumentNullException(nameof(version));

        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await version.FetchAvailableDownloads();
        }
    }

    private async void _DownloadVersionCommandExecute()
    {
        using (IDisposable _ = _updateVersionsBusyOperation.StartOperation())
        {
            Result result = await SelectedDownload!.DownloadAsync();
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowErrorDialog(result);
            }
        }
    }

    private void _LaunchVersionCommandExecute()
    {
        _selectedDownload!.Launch();
    }
}