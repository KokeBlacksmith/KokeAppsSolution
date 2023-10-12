using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Threading;
using KB.AvaloniaCore.Controls.Log;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.ConsoleCompanion.CommandView;

internal class CommandViewModel : BaseViewModel
{
    private ObservableCollection<LogMessage> _logMessages;

    public CommandViewModel()
    {
        _logMessages = new ObservableCollection<LogMessage>();
        TestMessagesPerformance();
    }

    public ObservableCollection<LogMessage> LogMessages
    {
        get { return _logMessages; }
        set { m_SetProperty(ref _logMessages, value); }
    }


    private void TestMessagesPerformance()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                Random random = new Random();
                LogMessage.SeverityLevel randomSeverity = (LogMessage.SeverityLevel)random.Next(0, 4);
                await Task.Delay(50);

                Dispatcher.UIThread.Invoke(() =>
                {
                    LogMessages.Add(new LogMessage(
                        "Test message",
                        "Test extended message",
                        randomSeverity
                    ));
                });
            }
        })
        .ContinueWith((t) => {
            throw new Exception(t.Exception.Message);
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}