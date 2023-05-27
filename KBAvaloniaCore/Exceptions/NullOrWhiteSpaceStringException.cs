namespace KBAvaloniaCore.Exceptions;

public sealed class NullOrWhiteSpaceStringException : Exception
{
    public NullOrWhiteSpaceStringException(string fieldName) 
        : base( fieldName + "cannot be null or empty string. ")
    {
    }
}