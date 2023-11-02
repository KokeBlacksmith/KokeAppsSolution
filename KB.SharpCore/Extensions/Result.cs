using KB.SharpCore.Utils;

namespace KB.SharpCore.Extensions;

public readonly struct Result
{
    private Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Messages = null;
    }

    private Result(params string[] messages)
    {
        IsSuccess = false;
        Messages = messages;
    }

    public bool IsSuccess { get; }

    public bool IsFailure
    {
        get { return !IsSuccess; }
    }

    public string[]? Messages { get; }

    public string? MessagesAsString
    {
        get
        {
            return CollectionHelper.StringArrayToNewLinesString(Messages);
        }
    }

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
        Messages = null;
    }

    private Result(params string[] messages)
    {
        Value = default(T);
        IsSuccess = false;
        Messages = messages;
    }

    public bool IsSuccess { get; }

    public bool IsFailure
    {
        get { return !IsSuccess; }
    }

    public T? Value { get; }

    public string[]? Messages { get; }

    public string? MessagesAsString
    {
        get 
        { 
            return CollectionHelper.StringArrayToNewLinesString(Messages);
        }
    }

    public Result ToResult()
    {
        return IsSuccess ? Result.CreateSuccess() : Result.CreateFailure(Messages);
    }
    
    public static Result<T> CreateFailure(params string[] errors)
    {
// #if DEBUG
//         System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
//         List<string> errorsList = new List<string>(messages.Length + 1);
//         errorsList.Add($"StackTrace: {t}");
//         messages = errorsList.Concat(messages).ToArray();
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