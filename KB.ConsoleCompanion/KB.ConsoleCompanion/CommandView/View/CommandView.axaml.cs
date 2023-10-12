using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KB.ConsoleCompanion.CommandView;

/// <summary>
/// View to send commands and display the output.
/// User can write commands that will be executed like in a CMD window.
/// </summary>
public partial class CommandView : UserControl
{
    public CommandView()
    {
        InitializeComponent();
    }
}