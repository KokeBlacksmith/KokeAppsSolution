namespace KBAvaloniaCore.Miscellaneous;

public readonly struct Result
{
    
    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Error = null;
    }
    
    public Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }
    
    public bool IsSuccess { get; }
    public bool IsFailure
    {
        get { return !IsSuccess; }
    }
    public string Error { get; }
}

public readonly struct Result<T>
{
    public Result(T value, bool isSuccess)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = null;
    }
    
    public Result(string error)
    {
        Value = default(T);
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure
    {
        get { return !IsSuccess; }
    }
    
    public T Value { get; }
    
    public string Error { get; }

    public Result ToResult()
    {
        return this.IsSuccess ? new Result(true) : new Result(this.Error);
    }
}