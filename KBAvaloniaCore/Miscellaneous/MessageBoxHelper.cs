using System.Text;
using System.Xml.XPath;

namespace KBAvaloniaCore.Miscellaneous;

public static class MessageBoxHelper
{
    public static void ShowErrorDialog(in Result result)
    {
        StringBuilder sb = new StringBuilder();
        if (result.Errors != null)
        {
            foreach (string error in result.Errors)
            {
                sb.AppendLine(error);
            }
        }

        MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Errors", sb.ToString()).Show();
    }
}