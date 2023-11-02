using Avalonia.Controls;
using Avalonia.Interactivity;

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

    private void _OnSendCommandClick(object sender, RoutedEventArgs args)
    {
        _SendUserCommand();
    }

    private void _OnCommandInputTextBoxSendsCommand(object sender, RoutedEventArgs args)
    {
        _SendUserCommand();
    }

    private void _SendUserCommand()
    {
        _commandInputTextBox.ConfirmOnReturnCommand?.Execute(_commandInputTextBox.Text);
        _commandInputTextBox.Text = String.Empty;
        _commandInputTextBox.Focus();
    }
}