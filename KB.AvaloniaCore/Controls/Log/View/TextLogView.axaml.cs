using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using KB.AvaloniaCore.Controls.Log;

namespace KB.AvaloniaCore.Controls;

internal partial class TextLogView : UserControl, ILogView
{
    public TextLogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _smartTextBox = this.FindControl<SmartTextBox>(nameof(TextLogView._smartTextBox));
    }

    public void AddMessage(LogMessage message)
    {
        _smartTextBox.AppendLine(message.Message);
    }

    public void AddMessages(IEnumerable<LogMessage> messages)
    {
        _smartTextBox.AppendLines(messages.Select(m => m.Message));
    }

    public void Clear()
    {
        _smartTextBox.Clear();
    }
}