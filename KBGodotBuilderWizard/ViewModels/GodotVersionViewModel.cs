using System.IO;
using Avalonia.Collections;
using KBAvaloniaCore.Miscellaneous;
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
}