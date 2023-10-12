using KB.AvaloniaCore.Controls.Log;

namespace KB.AvaloniaCore.Controls;

internal interface ILogView
{
    void AddMessage(LogMessage message);
    void AddMessages(IEnumerable<LogMessage> messages);
    
    void Clear();
}