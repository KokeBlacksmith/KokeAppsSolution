using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using KBAvaloniaCore.MessageBox;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotVersionHubViewModel : BaseViewModel, IReactiveModel<GodotVersionHub>
{
    private readonly BusyOperation _updateVersionsBusyOperation;
    private GodotExecutableViewModel? _selectedDownload;
    private GodotVersionViewModel? _selectedVersion;
    private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
    private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();
    private Path? _installsPath;

    public GodotVersionHubViewModel()
    {
        _updateVersionsBusyOperation = new BusyOperation(this, nameof(GodotVersionHubViewModel.IsUpdatingVersions));

        RefreshAvailableVersionsCommand = ReactiveCommand.Create(_FetchVersions);

        IObservable<bool> canDownload = this.WhenAnyValue(x => x.IsUpdatingVersions, y => y.SelectedDownload, (isUpdating, selected) => !isUpdating && selected != null);
        DownloadVersionCommand = ReactiveCommand.Create(_DownloadVersionCommandExecute, canDownload);
        IObservable<bool> canInstall = this.WhenAnyValue(x => x.IsUpdatingVersions, y => y.SelectedDownload, (isUpdating, selected) => !isUpdating && selected is { IsInstalled: true });
        LaunchVersionCommand = ReactiveCommand.Create(_LaunchVersionCommandExecute, canInstall);
        
        Model = new GodotVersionHub();
        _CheckForInstallsPath();
    }
    
    public GodotVersionHub Model { get; }

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
    
    public string InstallsPath
    {
        get { return Path.FullPathOrEmpty(_installsPath); }
        set
        {
            if (value == null)
            {
                _installsPath = null;
                this.RaisePropertyChanged();
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _installsPath, new Path(value));
            }
        }
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
        {
            throw new ArgumentNullException(nameof(version));
        }

        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            await version.FetchAvailableDownloads();
        }
    }

    private async void _DownloadVersionCommandExecute()
    {
        if (_installsPath == null)
        {
            MessageBoxHelper.ShowMessageDialog("No installs path has been provided");
            return;
        }
        
        if (!_installsPath!.Exists())
        {
            MessageBoxHelper.ShowMessageDialog("The selected installs path does not exist.");
            return;
        }
        
        using (IDisposable _ = _updateVersionsBusyOperation.StartOperation())
        {
            Result result = await SelectedDownload!.DownloadAsync(_installsPath);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowResultMessageDialog(result);
            }
        }
    }

    private void _LaunchVersionCommandExecute()
    {
        _selectedDownload!.Launch();
    }
    
    private async void _CheckForInstallsPath()
    {
        using (IDisposable _ = m_busyOperation.StartOperation())
        {
            bool success = false;
            await Task.Run(async () =>
            {
                Result result = await Model.DeserializeAsync();
                if (result.IsSuccess)
                {
                    success = true;
                    this.FromModel(Model);
                }
            });

            if (success)
            {
                return;
            }

            // Ask for a path where the installs will be located
            EMessageBoxButtonResult mbResult = await MessageBoxHelper.ShowResultMessageDialog("Installs path missing", "Do you want to go to configuration and set the new installs path?.", EMessageBoxButton.OkCancel);
            if (mbResult == EMessageBoxButtonResult.Ok)
            {
                                    
            }
        }
    }

    public void FromModel(GodotVersionHub model)
    {
        throw new NotImplementedException();
    }

    public void UpdateModel()
    {
        throw new NotImplementedException();
    }
}