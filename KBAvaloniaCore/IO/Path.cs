using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace KBAvaloniaCore.IO;

[Serializable]
public sealed class Path : ISerializable
{
    private string _path;
    private EPathType _pathType;
    
    
    public Path(string path, EPathType pathType)
    {
        _path = path;
        _pathType = pathType;
    }
    
    public Path(string path)
    {
        _path = path;
        _pathType = System.IO.Path.HasExtension(_path) ? EPathType.File : EPathType.Directory;
    }
    
    /// <summary>
    /// ISerialization constructor
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected Path(SerializationInfo info, StreamingContext context)
    {
        _path = (string)info.GetValue("Path", typeof(string));
        _pathType = (EPathType)info.GetValue("Member2", typeof(EPathType));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params string[] paths)
    {
        return new Path(System.IO.Path.Combine(paths));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists()
    {
        return _pathType == EPathType.Directory ? System.IO.Directory.Exists(_path) : System.IO.File.Exists(_path);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(string path)
    {
        _path = System.IO.Path.Combine(_path, path);
        _pathType = System.IO.Path.HasExtension(_path) ? EPathType.File : EPathType.Directory;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Combine(KBAvaloniaCore.IO.Path path)
    {
        this.Combine(path.GetPath());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetPath()
    {
        return _path;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFileName()
    {
        return _pathType == EPathType.File ? System.IO.Path.GetFileName(_path) : String.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetDirectoryName()
    {
        return System.IO.Path.GetDirectoryName(_path);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetExtension()
    {
        return System.IO.Path.GetExtension(_path);
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
                System.IO.File.Delete(_path);
            }
            else
            {
                return true;
            }
        }

        FileStream fs = System.IO.File.Create(_path);
        fs.Dispose();

        return this.Exists();
    }
    
    
    public override string ToString()
    {
        return $"Type ´{_pathType}´ Path ´{_path}´";
    }

    /// <summary>
    /// ISerializable implementation
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Path", _path);
        info.AddValue("PathType", Enum.GetName(typeof(EPathType), _pathType));
    }

    public static Path operator +(Path path1, Path path2) => Path.Combine(path1.GetPath(), path2.GetPath());
    public static implicit operator Path(string a) => new Path(a);
    public static explicit operator string(Path a) => a?.GetPath() ?? String.Empty;
}