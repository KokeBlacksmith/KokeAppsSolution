using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.MessageBox;
using KBGodotBuilderWizard.Enums;

namespace KBGodotBuilderWizard.Models;

[DataContract]
public class GodotExecutable
{
    private GodotExecutable()
    {
        
    }

    public GodotExecutable(string version, string fileName, string urlParentFolderName)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        UrlParentFolderName = urlParentFolderName;
        _Initialize();
    }

    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public string FileName { get; set; }
    [DataMember]
    public EOperatingSystem OperatingSystem { get; set; }
    [DataMember]
    public EProcessor Processor { get; set; }
    [DataMember]
    public string? InstallPath { get; set; }
    [DataMember]
    public string UrlParentFolderName { get; set; }
    [DataMember]
    public bool IsMonoVersion { get; set; }
    [DataMember]
    public bool IsInstalled { get; set; }

    public string GetPartialUrl()
    {
        return $"{Version}{UrlParentFolderName}/{FileName}.zip".Replace('\\', '/');
    }

    private void _Initialize()
    {
        IsMonoVersion = FileName.Contains("mono");

        if (FileName.Contains("64"))
        {
            Processor = EProcessor.X64;
        }
        else if (FileName.Contains("32") || FileName.Contains("86"))
        {
            Processor = EProcessor.X86;
        }
        else
        {
            Processor = EProcessor.ARM;
        }

        if (FileName.Contains("win"))
        {
            OperatingSystem = EOperatingSystem.Windows;
        }
        else if (FileName.Contains("osx") || FileName.Contains("macos"))
        {
            OperatingSystem = EOperatingSystem.MacOS;
        }
        else if (FileName.Contains("linux_server"))
        {
            OperatingSystem = EOperatingSystem.LinuxServer;
        }
        else if (FileName.Contains("x11") || FileName.Contains("linux"))
        {
            OperatingSystem = EOperatingSystem.Linux;
        }
        else if (FileName.Contains("web"))
        {
            OperatingSystem = EOperatingSystem.Web;
        }
    }

    public Task<Result> DownloadAsync(Path installsPath)
    {
        if (installsPath == null)
        {
            throw new ArgumentNullException(nameof(installsPath));
        }
        
        return Task.Run(async () =>
        {
            string fileName = FileName;
            if (System.IO.Path.HasExtension(FileName))
            {
                // Replace extension dot by underscore
                string extension = System.IO.Path.GetExtension(FileName);
                if (extension.Length <= 3)
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(FileName);
                    fileName = fileNameWithoutExtension + "_" + System.IO.Path.GetExtension(FileName).TrimStart('.');
                }
            }

            // string fileName = FileName.Replace('.', '_');
            Path versionInstallPath = Path.Combine(installsPath.FullPath, Version.Replace('.', '_'));
            Path installFolderPath = Path.Join(versionInstallPath, fileName);
            installFolderPath = installFolderPath.ConvertToDirectory();
            // Delete previous install if existed
            installFolderPath.DeleteDirectory(true);
            versionInstallPath.CreateDirectory();

            fileName += ".zip";
            Path zipFilePath = Path.Join(versionInstallPath, fileName);
            Result result = await GodotVersionFetcher.DownloadVersion(zipFilePath, GetPartialUrl());

            if (result.IsFailure)
            {
                return result;
            }

            ZipFile.ExtractToDirectory(zipFilePath.FullPath, installFolderPath.FullPath, true);
            zipFilePath.DeleteFile();

            if (this.IsMonoVersion)
            {
                // If it is mono, the zip contains another folder with the same name as the godot version file
                // We have to move the files from that folder to the install folder
                //Rename install folder because it has the same name as the folder that it contains
                Result<Path> tmpInstallPathResult = FileSystem.RenameDirectory(installFolderPath, $"{FileName}_tmp");
                if (tmpInstallPathResult.IsFailure)
                {
                    return tmpInstallPathResult.ToResult();
                }

                // Move the files from the install folder path to the version path
                Result moveResult = FileSystem.MoveFilesAndDirectories(tmpInstallPathResult.Value, versionInstallPath);
                if (moveResult.IsFailure)
                {
                    return moveResult;
                }

                tmpInstallPathResult.Value.DeleteDirectory(true);

                // Mono version zip name comes without the extension of the executable, so we have to add it to the FileName property
                // It depends in the OS that we are at this moment
                if (OperatingSystem == EOperatingSystem.Windows)
                {
                    FileName += ".exe";
                }
                else if (OperatingSystem == EOperatingSystem.MacOS)
                {
                    FileName += ".app";
                }
            }

            Path godotExecutablePath = Path.Join(installFolderPath, this.FileName);
            this.IsInstalled = true;
            this.InstallPath = godotExecutablePath.FullPath;
            return Result.CreateSuccess();
        });
    }

    public Result Uninstall()
    {
        return Result.CreateSuccess();
    }

    public Result Launch()
    {
        Path installPath = new Path(this.InstallPath);
        if (!IsInstalled || !installPath.Exists())
        {
            return Result.CreateFailure("Godot executable is not installed");
        }

        using (Process godotApp = new Process())
        {
            godotApp.StartInfo.UseShellExecute = false;
            godotApp.StartInfo.FileName = this.InstallPath;
            godotApp.StartInfo.CreateNoWindow = true;
            godotApp.Start();
            return Result.CreateSuccess();
        }
    }
}