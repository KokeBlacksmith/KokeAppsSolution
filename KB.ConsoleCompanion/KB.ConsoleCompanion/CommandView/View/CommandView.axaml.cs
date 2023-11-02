using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using ConsoleCompanionAPI.Data;
using DynamicData.Binding;
using System.Collections.Specialized;

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
        CommandViewModel viewModel = (CommandViewModel)DataContext!;
        viewModel.CommandsCollection.Add(new ConsoleCommand("Welcome to Console Companion!", ConsoleCommand.ECommandType.Info));
        if(viewModel.CommandsCollection is INotifyCollectionChanged observableCollection)
        {
            observableCollection.CollectionChanged += _OnCommandsCollectionChanged;
        }
    }

    private void _OnCommandsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if(_listBox.ItemCount > 0)
        {
            _listBox.ScrollIntoView(_listBox.Items[^1]!);
        }
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