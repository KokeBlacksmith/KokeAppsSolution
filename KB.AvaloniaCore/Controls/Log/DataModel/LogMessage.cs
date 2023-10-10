namespace KB.AvaloniaCore.Controls.Log;

public struct LogMessage
{
    public enum SeverityLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal,
    }
    
    public LogMessage(string message, string? extendedMessage, SeverityLevel severity)
    {
        Message = message;
        ExtendedMessage = extendedMessage;
        Severity = severity;
        TimeStamp = DateTime.UtcNow;
    }
    
    public string Message { get; }
    public string? ExtendedMessage { get; } = null;
    public SeverityLevel Severity { get; } = SeverityLevel.Info;
    public DateTime TimeStamp { get; }
}