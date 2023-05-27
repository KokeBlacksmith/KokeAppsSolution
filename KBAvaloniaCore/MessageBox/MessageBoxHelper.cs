using System.Text;
using KBAvaloniaCore.Exceptions;
using MessageBox.Avalonia;

namespace KBAvaloniaCore.MessageBox;

public static class MessageBoxHelper
{
    public static async Task<EMessageBoxButtonResult> ShowResultMessageDialog(string header, string message, EMessageBoxButton button = EMessageBoxButton.Ok)
    {
        if (String.IsNullOrWhiteSpace(header))
        {
            throw new NullOrWhiteSpaceStringException(nameof(header));
        }

        if (String.IsNullOrWhiteSpace(message))
        {
            throw new NullOrWhiteSpaceStringException(nameof(message));
        }

        var buttonResult = await MessageBoxManager.GetMessageBoxStandardWindow(header, message, button.ToButtonEnum()).Show();
        return buttonResult.ToEMessageBoxButtonResult();
    }
    
    public static void ShowResultMessageDialog(string header, in Result result)
    {
        if (String.IsNullOrWhiteSpace(header))
        {
            throw new NullOrWhiteSpaceStringException(nameof(header));
        }

        StringBuilder sb = new StringBuilder();
        if (String.IsNullOrEmpty(header))
        {
            sb.AppendLine(header + "\n");
        }

        if (result.Messages?.Any() ?? false)
        {
            foreach (string message in result.Messages)
            {
                sb.AppendLine(message);
            }
        }

        MessageBoxManager.GetMessageBoxStandardWindow(header, sb.ToString()).Show();
    }
    
    public static void ShowResultMessageDialog(in Result result)
    {
        if (result.Messages != null && result.Messages.Any())
        {
            MessageBoxHelper.ShowMessageDialog(result.Messages);
        }
        else
        {
            MessageBoxHelper.ShowMessageDialog(new[] {
                result.IsSuccess ? "Success" : "Failure",
            });
        }
    }

    public static void ShowMessageDialog(params string[] errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        StringBuilder sb = new StringBuilder();
        foreach (string error in errors)
        {
            sb.AppendLine(error);
        }

        MessageBoxManager.GetMessageBoxStandardWindow("Messages", sb.ToString()).Show();
    }
}