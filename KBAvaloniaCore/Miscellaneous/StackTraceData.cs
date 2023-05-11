using System.Diagnostics;
using Avalonia.Rendering;

namespace KBAvaloniaCore.Miscellaneous;

public class StackTraceData
{
    private readonly StackTrace _stackTrace;
    private readonly StackFrame _stackFrame;
    public StackTraceData()
    {
        _stackTrace = new StackTrace();
        _stackFrame = _stackTrace.GetFrame(0);
    }
    
    public StackTraceData(Exception ex)
    {
        _stackTrace = new StackTrace(ex);
        _stackFrame = _stackTrace.GetFrame(0);
    }

    public string? FileName
    {
        get {return _stackFrame.GetFileName(); }
    }
    
    public int LineNumber
    {
        get {return _stackFrame.GetFileLineNumber(); }
    }
    
    public int ColumnNumber
    {
        get {return _stackFrame.GetFileColumnNumber(); }
    }
    
    public string? MethodName
    {
        get {return _stackFrame.GetMethod()?.Name; }
    }
    
    public string? ClassName
    {
        get {return _stackFrame.GetMethod()?.DeclaringType?.Name; }
    }
    
    public string? Namespace
    {
        get {return _stackFrame.GetMethod()?.DeclaringType?.Namespace; }
    }
    
    public string? FullClassName
    {
        get {return $"{Namespace}.{ClassName}"; }
    }
    
    public string? FullMethodName
    {
        get {return $"{FullClassName}.{MethodName}"; }
    }
    
    public string? FullNamespace
    {
        get {return $"{Namespace}.{ClassName}.{MethodName}"; }
    }
    
    public string? FullFileName
    {
        get {return $"{FileName}"; }
    }

    public string? FullFilePath
    {
        get { return $"{FileName}:{LineNumber}"; }
    }
        
    public string? FullFilePathWithLine
    {
        get { return $"{FileName}:{LineNumber}:{ColumnNumber}"; }
    }
    
    public override string ToString()
    {
        return $"Class: {FullClassName}"
               + $"\nMethod: {FullMethodName}"
               + $"\nFileName:Line:Column: {FullFilePathWithLine}"
               + $"\nStackTrace: {_stackTrace.ToString()}";
    }
}