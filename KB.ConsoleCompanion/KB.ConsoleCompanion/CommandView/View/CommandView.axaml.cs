using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KB.AvaloniaCore.Controls;
using KB.AvaloniaCore.Controls.Log;
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

    private readonly ICommand _addCommandLineCommand;

    public CommandView()
    {
        InitializeComponent();
        _viewModel = DataContext as CommandViewModel ?? throw new NullReferenceException(nameof(_viewModel));
        _addCommandLineCommand = _viewModel.AddCommandLineCommand;
    }

    private void _OnSendCommandClick(object sender, RoutedEventArgs args)
    {
        ConsoleCommand newCommand = new ConsoleCommand(_commandInputTextBox.Text!, ConsoleCommand.ECommandType.UserInput);
        _addCommandLineCommand?.Execute(newCommand);
        _commandInputTextBox.Text = String.Empty;

        _commandInputTextBox.Focus();
    }
}