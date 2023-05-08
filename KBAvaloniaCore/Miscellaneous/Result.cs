namespace KBAvaloniaCore.Miscellaneous;

public readonly struct Result
{
    private Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Errors = null;
    }

    private Result(params string[] errorses)
    {
        IsSuccess = false;
        Errors = errorses;
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
        return new Result(exception.Message, exception.InnerException?.Message);
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

    public static Result<T> CreateFailure(string[] errors)
    {
        return new Result<T>(errors);
    }

    public static Result<T> CreateFailure(Exception exception)
    {
        return new Result<T>(exception.Message, exception.InnerException?.Message);
    }

    public static Result<T> CreateSuccess(T value)
    {
        return new Result<T>(value, true);
    }
}