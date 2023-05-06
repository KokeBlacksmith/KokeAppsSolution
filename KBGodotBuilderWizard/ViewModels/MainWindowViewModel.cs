using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using JetBrains.Annotations;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using KBGodotBuilderWizard.Views;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private AvaloniaList<GodotVersionViewModel> _versionsList = new AvaloniaList<GodotVersionViewModel>();
        private AvaloniaList<GodotVersionViewModel> _selectedVersionDownloadList = new AvaloniaList<GodotVersionViewModel>();
        private GodotVersionViewModel _selectedVersion;
        private string _selectedDownload;
        private int _totalInstalls;
        
        public MainWindowViewModel()
        {
            RefreshAvailableVersionsCommand = ReactiveCommand.Create(_FetchVersions);
            InitializeVersionConfigurationCommand = ReactiveCommand.Create(_InitializeVersionConfigurationCommandExecute);
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
        
        public GodotVersionViewModel SelectedVersion
        {
            get { return _selectedVersion; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedVersion, value);
                _FetchVersionDownloads(_selectedVersion);
            }
        }
        
        public string SelectedDownload
        {
            get { return _selectedDownload; }
            set { this.RaiseAndSetIfChanged(ref _selectedDownload, value); }
        }
        
        public int TotalInstalls
        {
            get { return _totalInstalls; }
            set { this.RaiseAndSetIfChanged(ref _totalInstalls, value); }
        }

        public ICommand RefreshAvailableVersionsCommand { get; }
        public ICommand InitializeVersionConfigurationCommand { get; }

        /// <summary>
        /// Retrieves the available number versions
        /// </summary>
        private async void _FetchVersions()
        {
            this.IsBusy = true;
            IEnumerable<GodotVersionViewModel> godotVersions = (await GodotVersionFetcher.FetchVersions())
                                                                        .Select(strVersion => new GodotVersionViewModel(strVersion)); 
            VersionsList = new AvaloniaList<GodotVersionViewModel>(godotVersions);
            this.IsBusy = false;
        }
        
        private async void _FetchVersionDownloads([NotNull]GodotVersionViewModel version, string extendPath = "/")
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
        }
        
        private void _InitializeVersionConfigurationCommandExecute()
        {
            // KBAvaloniaCore.IO.Path configurationFile = KBAvaloniaCore.IO.Path.Combine(System.IO.Path.GetTempPath(), Assembly.GetCallingAssembly().GetName().Name!, "SaveDataConfiguration.json");
            // configurationFile.CreateDirectory();
            // configurationFile.CreateFile(false);
            //
            // ConfigurationWindow configurationWidow = new ConfigurationWindow();
            // configurationWidow.ShowDialog(this);
        }
    }
}