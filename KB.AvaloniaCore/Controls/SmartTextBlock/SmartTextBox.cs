using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace KB.AvaloniaCore.Controls;

public class SmartTextBox : TextBox
{
    // So this class is a TextBox with some extra functionality.
    // Without this, the SmartTextBox would not be rendered.
    protected override Type StyleKeyOverride { get { return typeof(TextBox); } }
    
    public void Clear()
    {
        Text = String.Empty;
    }
    
    public void AppendLine(string line)
    {
        Text += Environment.NewLine + line;
    }
    
    public void AppendLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            AppendLine(line);
        }
    }
}