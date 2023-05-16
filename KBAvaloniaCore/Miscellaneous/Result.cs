using System.Runtime.CompilerServices;

namespace KBAvaloniaCore.Miscellaneous;

public readonly struct Result
{
    private Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Errors = null;
    }

    private Result(params string[] errors)
    {
        IsSuccess = false;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure
    {
        get { return !IsSuccess; }
    }

    public string[]? Errors { get; }

    public static Result CreateFailure(params string[] errors)
    {
        return new Result(errors);
    }

    public static Result CreateFailure(Exception exception)
    {
        StackTraceData stackTraceData = new StackTraceData(exception);
        return Result.CreateFailure(stackTraceData.ToString()!, exception.Message, exception.InnerException?.Message ?? String.Empty );
    }

    public static Result CreateSuccess()
    {
        return new Result(true);
    }
}

public readonly struct Result<T>
{
    private Result(T value, bool isSuccess)
    {
        Value = value;
        IsSuccess = isSuccess;
        Errors = null;
    }

    private Result(params string[] errors)
    {
        Value = default(T);
        IsSuccess = false;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure
    {
        get { return !IsSuccess; }
    }

    public T Value { get; }

    public string[] Errors { get; }

    public Result ToResult()
    {
        return IsSuccess ? Result.CreateSuccess() : Result.CreateFailure(Errors);
    }
    
    public static Result<T> CreateFailure(params string[] errors)
    {
// #if DEBUG
//         System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
//         List<string> errorsList = new List<string>(errors.Length + 1);
//         errorsList.Add($"StackTrace: {t}");
//         errors = errorsList.Concat(errors).ToArray();
// #endif
        
        return new Result<T>(errors);
    }

    public static Result<T> CreateFailure(Exception exception)
    {
        System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace(exception);
        return Result<T>.CreateFailure(t.ToString(), exception.Message, exception.InnerException?.Message ?? String.Empty);
    }

    public static Result<T> CreateSuccess(T value)
    {
        return new Result<T>(value, true);
    }
}