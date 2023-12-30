using System.Runtime.CompilerServices;
using KB.SharpCore.Utils;

namespace KB.SharpCore.IO;

public static class FileSystem
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<Path> RenameDirectory(Path directoryPath, string newName)
    {
        if (directoryPath.IsFile)
        {
            return Result<Path>.CreateFailure($"Can't rename directory '{directoryPath.GetDirectoryName()}'. It is a file.");
        }
        
        if (!directoryPath.Exists())
        {
            return Result<Path>.CreateFailure($"Can't rename directory '{directoryPath.GetDirectoryName()}'. It does not exist.");
        }

        string directoryName = directoryPath.GetShortDirectoryName()!;
        string finalPathString = directoryPath.FullPath.Replace(directoryName, newName);
        System.IO.Directory.Move(directoryPath.FullPath, finalPathString);
        Path finalPath = new Path(finalPathString);
        if (finalPath.Exists())
        {
            return Result<Path>.CreateSuccess(new Path(finalPathString));
        }
        else
        {
            return Result<Path>.CreateFailure($"An error occurred on renaming the directory {directoryPath.FullPath} to {finalPathString}.");
        }
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result MoveFilesAndDirectories(in Path sourceDirectoryPath, in Path destinationDirectoryPath)
    {
        if (!sourceDirectoryPath.IsDirectory)
        {
            return Result.CreateFailure($"The source path '{sourceDirectoryPath.FullPath}' is not a directory");
        }
        
        if (!destinationDirectoryPath.IsDirectory)
        {
            return Result.CreateFailure($"The destination path '{destinationDirectoryPath.FullPath}' is not a directory");
        }
        
        string? startErrorMessage = null;
        if (!destinationDirectoryPath.Exists())
        {
            startErrorMessage = $"Destination directory does not exists.";
        }

        if (startErrorMessage != null && !sourceDirectoryPath.Exists())
        {
            startErrorMessage = $"Source directory does not exists.";
        }

        if (startErrorMessage != null)
        {
            string errorMessage = $"Error moving files from '{sourceDirectoryPath.FullPath}' to '{destinationDirectoryPath.FullPath}'.";
            return Result.CreateFailure($"{startErrorMessage} {errorMessage}");
        }
        
        try
        {
            // Move all files from the source directory to the destination directory
            foreach (string filePath in GetFilesInDirectory(sourceDirectoryPath))
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string destinationFilePath = System.IO.Path.Combine(destinationDirectoryPath.FullPath, fileName);
                System.IO.File.Move(filePath, destinationFilePath);
            }

            // Move all subdirectories from the source directory to the destination directory
            foreach (Path subdirectoryPath in GetDirectories(sourceDirectoryPath))
            {
                string subdirectoryName = subdirectoryPath.GetShortDirectoryName()!;
                Path destinationSubdirectoryPath = Path.Join(destinationDirectoryPath, subdirectoryName);
                System.IO.Directory.Move(subdirectoryPath.FullPath, destinationSubdirectoryPath.FullPath);
            }
            
            return Result.CreateSuccess();
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DirectoryInfo CreateDirectory(Path path)
    {
        return Directory.CreateDirectory(path.GetDirectoryName()!);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DeleteDirectory(Path path, bool recursive)
    {
        if (Directory.Exists(path.FullPath))
        {
            Directory.Delete(path.FullPath, recursive);
        }

        return Directory.Exists(path.FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result DeleteFile(Path path)
    {
        try
        {
            if (!path.IsFile)
            {
                return Result.CreateFailure($"The path '{path.FullPath}' is not a file");
            }

            File.Delete(path.FullPath);
            return Result.CreateSuccess();
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CreateFile(Path path, bool overrideExisting)
    {
        if (!path.IsFile)
        {
            throw new Exception("Path is not a file");
        }

        if (path.Exists())
        {
            if (overrideExisting)
            {
                File.Delete(path.FullPath);
            }
            else
            {
                return true;
            }
        }

        using FileStream fs = File.Create(path.FullPath);
        return path.Exists();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> GetDirectories(Path path)
    {
        return System.IO.Directory.GetDirectories(path.FullPath).Select(dir => dir + System.IO.Path.DirectorySeparatorChar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> GetFilesInDirectory(Path path)
    {
        return System.IO.Directory.GetFiles(path.GetDirectoryName() ?? String.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRootDirectoryName(Path path)
    {
        return Directory.GetDirectoryRoot(path.FullPath);
    }
}