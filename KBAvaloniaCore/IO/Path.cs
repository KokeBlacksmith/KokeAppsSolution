using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Avalonia.Rendering;

namespace KBAvaloniaCore.IO;

[Serializable]
[DataContract(Name = nameof(Path))]
public sealed class Path
{
    private EPathType _pathType;
    private string _fullPath;
    
    public Path(string path)
    {
        FullPath = path;
    }

    /// <summary>
    /// Constructor using for serialization
    /// </summary>
    private Path() { }

    [DataMember(Name = nameof(Path.FullPath))]
    public string FullPath
    {
        get { return _fullPath;}
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
        return _pathType == EPathType.Directory ? System.IO.Directory.Exists(FullPath) : System.IO.File.Exists(FullPath);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(string path)
    {
        FullPath = System.IO.Path.Combine(FullPath, path);
        _pathType = System.IO.Path.HasExtension(FullPath) ? EPathType.File : EPathType.Directory;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(KBAvaloniaCore.IO.Path path)
    {
        this.Combine(path.GetPath());
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
        return System.IO.Directory.CreateDirectory(this.GetDirectoryName());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CreateFile(bool overrideExisting)
    {
        if (_pathType != EPathType.File)
        {
            throw new Exception("Path is not a file");
        }

        if (this.Exists())
        {
            if (overrideExisting)
            {
                System.IO.File.Delete(FullPath);
            }
            else
            {
                return true;
            }
        }

        FileStream fs = System.IO.File.Create(FullPath);
        fs.Dispose();

        return this.Exists();
    }
    
    
    public override string ToString()
    {
        return $"Type ´{_pathType}´ Path ´{FullPath}´";
    }

    public static Path operator +(Path path1, Path path2) => Path.Combine(path1.GetPath(), path2.GetPath());
    public static implicit operator Path(string a) => new Path(a);
    public static explicit operator string(Path a) => a?.GetPath() ?? String.Empty;
}