using Avalonia.Controls;
using Avalonia.Interactivity;
using ConsoleCompanionAPI.Data;
using System.Collections.Specialized;

namespace KB.ConsoleCompanion.CommandView;

/// <summary>
/// View to send commands and display the output.
/// User can write commands that will be executed like in a CMD window.
/// </summary>
public partial class CommandView : UserControl
{
    static CommandView()
    {

    }

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

    private void _OnAddMacroButtonClick(object sender, RoutedEventArgs args)
    {
        //Window graphWindow = new Window();
        //GraphCanvas graphCanvas = new GraphCanvas();
        //graphCanvas.ChildNodes = new AvaloniaList<Node>() { 
        //    new TimerNode() { 
        //        Width = 100, Height= 100, Background = Brushes.Aqua, PositionX=50, PositionY=200, 
        //        CornerRadius = new CornerRadius(10d), Padding = new Thickness(10d), BorderThickness = new Thickness(2d), BorderBrush = Brushes.Yellow,
        //    },
        //    new TimerNode() {
        //        Width = 100, Height= 100, Background = Brushes.Aqua, PositionX=150, PositionY=200,
        //        CornerRadius = new CornerRadius(10d), Padding = new Thickness(10d), BorderThickness = new Thickness(2d), BorderBrush = Brushes.Purple,
        //    },
        //};
        //graphWindow.Content = graphCanvas;
        //graphWindow.Show();
    }

    private void _OnSendCommandClick(object sender, RoutedEventArgs args)
    {
        _SendUserCommand();
    }

    private void _OnCommandInputTextBoxSendsCommand(object sender, RoutedEventArgs args)
    {
        _SendUserCommand();
    }

    private void _OnCommandsListBoxItemDoubleTapped(object sender, RoutedEventArgs args)
    {
        Control senderControl = (Control)sender;
        ListBoxItem listBoxItem = (ListBoxItem)senderControl.Parent!;
        _commandInputTextBox.Text = ((ConsoleCommand)listBoxItem.DataContext!).Command;
        _commandInputTextBox.Focus();
        _commandInputTextBox.UnselectAllText();
    }

    private void _SendUserCommand()
    {
        _commandInputTextBox.ConfirmOnReturnCommand?.Execute(_commandInputTextBox.Text);
        _commandInputTextBox.Text = String.Empty;
        _commandInputTextBox.Focus();
    }

}