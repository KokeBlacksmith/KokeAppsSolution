using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace KBAvaloniaCore.IO;

[DataContract(Name = nameof(Path))]
public sealed class Path
{
    private string _fullPath;
    private EPathType _pathType;

    public Path(string path)
    {
        FullPath = path;
    }

    /// <summary>
    ///     Constructor using for serialization
    /// </summary>
    private Path() { }

    [DataMember]
    public string FullPath
    {
        get { return _fullPath; }
        set
        {
            _fullPath = value;
            _pathType = System.IO.Path.HasExtension(FullPath) ? EPathType.File : EPathType.Directory;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params string[] paths)
    {
        return new Path(System.IO.Path.Combine(paths));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists()
    {
        return _pathType == EPathType.Directory ? Directory.Exists(FullPath) : File.Exists(FullPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(string path)
    {
        FullPath = System.IO.Path.Combine(FullPath, path);
        _pathType = System.IO.Path.HasExtension(FullPath) ? EPathType.File : EPathType.Directory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(Path path)
    {
        Combine(path.GetPath());
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
    public string GetDirectoryName()
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


    public override string ToString()
    {
        return $"Type ´{_pathType}´ Path ´{FullPath}´";
    }

    public static Path operator +(Path path1, Path path2)
    {
        return Path.Combine(path1.GetPath(), path2.GetPath());
    }

    public static implicit operator Path(string a)
    {
        return new Path(a);
    }

    public static explicit operator string(Path a)
    {
        return a?.GetPath() ?? String.Empty;
    }
}