using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KB.AvaloniaCore.IO;

namespace KBGodotBuilderWizard.Models;

[DataContract]
public class GodotVersion
{
    public GodotVersion(string version)
    {
        Version = version;
    }
    
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public List<GodotExecutable> Executables { get; set; } = new List<GodotExecutable>();
    
    public Task FetchAvailableDownloads()
    {
        return _FetchAvailableDownloads(null);
    }
    
    private Task _FetchAvailableDownloads(Path? extendPath)
    {
        return Task.Run(async () =>
        {
            string urlParentFolderName = extendPath?.GetDirectoryName() ?? System.IO.Path.DirectorySeparatorChar.ToString();
            IEnumerable<GodotVersionFetcher.GodotInstallData> installsData = await GodotVersionFetcher.FetchVersionDownloadsAsync(this.Version + extendPath?.FullPath); 
            // Fetch from the web
            foreach (GodotVersionFetcher.GodotInstallData download in installsData)
            {
                if (!System.IO.Path.HasExtension(download.FileName))
                {
                    // It is a folder
                    //FileName contains the directories to the file
                    Path newExtendPath = Path.Join(urlParentFolderName, download.FileName);
                    await _FetchAvailableDownloads(newExtendPath);
                }
                else
                {
                    this._AddInstall(download.FileName, urlParentFolderName);
                }
            }
        });
    }
    
    private void _AddInstall(string fileName, string urlParentFolderName)
    {
        if (System.IO.Path.GetExtension(fileName) != ".zip")
            return;

        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
        GodotExecutable executable = new GodotExecutable(Version, fileNameWithoutExtension, urlParentFolderName);
        Executables.Add(executable);
    }
}