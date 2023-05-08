using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Collections;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotVersionViewModel : BaseViewModel
{
    private AvaloniaList<GodotInstallViewModel> _installs = new AvaloniaList<GodotInstallViewModel>();
    private string _version;

    public GodotVersionViewModel(string version)
    {
        Version = version;
    }

    public string Version
    {
        get { return _version; }
        set { this.RaiseAndSetIfChanged(ref _version, value); }
    }

    public AvaloniaList<GodotInstallViewModel> Installs
    {
        get { return _installs; }
        set { this.RaiseAndSetIfChanged(ref _installs, value); }
    }

    public void AddInstall(string fileName, string urlParentFolderName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        switch (Path.GetExtension(fileName))
        {
            case ".zip":
            {
                GodotInstallViewModel install = new GodotInstallViewModel(Version, fileNameWithoutExtension, urlParentFolderName);
                Installs.Add(install);
                break;
            }
            case ".txt":
            {
                break;
            }
        }
    }
    
    public Task FetchAvailableDownloads(string extendPath = "/")
    {
        return Task.Run(async () =>
        {
            string urlParentFolderName = Path.GetDirectoryName(extendPath)!;
            // Fetch from the web
            foreach (GodotVersionFetcher.GodotInstallData download in await GodotVersionFetcher.FetchVersionDownloads(this.Version + extendPath))
            {
                if (!Path.HasExtension(download.FileName))
                {
                    //FileName contains the directories to the file

                    // It is a folder
                    FetchAvailableDownloads($"{extendPath}{download.FileName}");
                }
                else
                {
                    this.AddInstall(download.FileName, urlParentFolderName);
                }
            }
        });
    }
}