using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using KBAvaloniaCore.Miscellaneous;

namespace KBAvaloniaCore.IO;

public readonly struct Path
{
    private readonly EPathType _pathType;

    public Path(string path) : this()
    {
        FullPath = path;
        _pathType = System.IO.Path.HasExtension(FullPath) ? EPathType.File : EPathType.Directory;
    }

    public string FullPath { get; }

    public bool TryGetParent(out Path parentPath)
    {
        DirectoryInfo? parent = Directory.GetParent(FullPath);
        if (parent != null)
        {
            parentPath = new Path(parent.FullName);
            return true;
        }

        parentPath = default(Path);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params string[] paths)
    {
        return new Path(System.IO.Path.Combine(paths));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params Path[] paths)
    {
        return new Path(System.IO.Path.Combine(paths.Select(p => p.FullPath).ToArray()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists()
    {
        return _pathType == EPathType.Directory ? Directory.Exists(FullPath) : File.Exists(FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetPath()
    {
        return FullPath;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFileName()
    {
        return _pathType == EPathType.File ? System.IO.Path.GetFileName(FullPath) : String.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetDirectoryName()
    {
        return System.IO.Path.GetDirectoryName(FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetExtension()
    {
        return System.IO.Path.GetExtension(FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DirectoryInfo CreateDirectory()
    {
        return Directory.CreateDirectory(GetDirectoryName());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DeleteDirectory(bool recursive)
    {
        if (Directory.Exists(FullPath))
            Directory.Delete(FullPath, recursive);

        return Directory.Exists(FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result DeleteFile()
    {
        try
        {
            if (_pathType != EPathType.File)
                return Result.CreateFailure($"The path '{FullPath}' is not a file");

            File.Delete(FullPath);
            return Result.CreateSuccess();
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CreateFile(bool overrideExisting)
    {
        if (_pathType != EPathType.File)
        {
            throw new Exception("Path is not a file");
        }

        if (Exists())
        {
            if (overrideExisting)
            {
                File.Delete(FullPath);
            }
            else
            {
                return true;
            }
        }

        FileStream fs = File.Create(FullPath);
        fs.Dispose();

        return Exists();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetRootDirectoryName()
    {
        return Directory.GetDirectoryRoot(FullPath);
    }


    public override string ToString()
    {
        return $"Type ´{_pathType}´ Path ´{FullPath}´";
    }


    public static Path operator +(Path path1, Path path2)
    {
        return Path.Combine(path1.GetPath(), path2.GetPath());
    }

    public static explicit operator Path(string a)
    {
        return new Path(a);
    }

    public static explicit operator string(Path a)
    {
        return a.GetPath() ?? String.Empty;
    }
}