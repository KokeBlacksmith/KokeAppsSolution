using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KB.AvaloniaCore.Controls;
using KB.AvaloniaCore.Controls.Log;
using KB.AvaloniaCore.ReactiveUI;
using KB.ConsoleCompanion.DataModels;
using KB.SharpCore.Utils;

namespace KB.ConsoleCompanion.CommandView;

/// <summary>
/// View to send commands and display the output.
/// User can write commands that will be executed like in a CMD window.
/// </summary>
public partial class CommandView : UserControl
{
    private readonly CommandViewModel _viewModel;

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