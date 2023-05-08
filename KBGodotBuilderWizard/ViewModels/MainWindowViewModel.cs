using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using JetBrains.Annotations;
using KBAvaloniaCore.Controls;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly BusyOperation _updateVersionsBusyOperation;
    private GodotInstallViewModel? _selectedDownload;
    private GodotVersionViewModel? _selectedVersion;
    private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
    private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();

    public MainWindowViewModel()
    {
        _updateVersionsBusyOperation = new BusyOperation(this, nameof(MainWindowViewModel.IsUpdatingVersions));

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
                IEnumerable<GodotVersionViewModel> godotVersions = (await GodotVersionFetcher.FetchVersions()).Select(strVersion => new GodotVersionViewModel(strVersion));
                VersionsList = new AvaloniaList<GodotVersionViewModel>(godotVersions);
            });
        }
    }

    private async void _FetchVersionDownloads([NotNull] GodotVersionViewModel version, string extendPath = "/")
    {
        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await version.FetchAvailableDownloads(extendPath);
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

    private async void _LaunchVersionCommandExecute()
    {
        _selectedDownload!.Launch();
    }
}