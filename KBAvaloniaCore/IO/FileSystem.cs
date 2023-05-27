using System.Runtime.CompilerServices;
using KBAvaloniaCore.MessageBox;

namespace KBAvaloniaCore.IO;

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
            foreach (string filePath in sourceDirectoryPath.GetFilesInDirectory())
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string destinationFilePath = System.IO.Path.Combine(destinationDirectoryPath.FullPath, fileName);
                System.IO.File.Move(filePath, destinationFilePath);
            }

            // Move all subdirectories from the source directory to the destination directory
            foreach (Path subdirectoryPath in sourceDirectoryPath.GetDirectories())
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
}