using Avalonia.Controls;
using Avalonia.Interactivity;
using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;

namespace KB.ConsoleCompanion;

public partial class MainConsoleCompanion : UserControl
{
    private readonly CommandView.CommandView _commandView;
    private readonly MacroEditView.MacroEditView _macroEditView;
    private readonly ConfigurationView.ConfigurationView _configurationView;

    IServerProtocolAPI _server;
    public MainConsoleCompanion()
    {
        //Test
        _InitServer();
        // End Test

        InitializeComponent();

        _commandView = new CommandView.CommandView();
        _macroEditView = new MacroEditView.MacroEditView();
        _configurationView = new ConfigurationView.ConfigurationView();
        _viewContainer.Content = _commandView;
    }

    private string[] _availableCommands = { 
        "Throw Error",
        "Throw Warning",
        "Throw Info",
        "Say Hello",
        "Say Goodbye",
        "Say How are you?",
    }; 

    private void _InitServer()
    {
        _server = ProtocolFactory.CreateServer();
        _server.Start("127.0.0.1", "55555");

        _server.OnCommandReceived += OnCommandReceived;
        _server.OnRequestAvailableCommandsReceived += OnRequestAvailableCommandsReceived;
    }

    private ConsoleCommand OnCommandReceived(ConsoleCommand clientCommand)
    {
        ConsoleCommand response = new ConsoleCommand($"Response from command '{clientCommand.Command}' received sucessfully!", ConsoleCommand.ECommandType.Info);
        return response;
    }

    private ConsoleCommand OnRequestAvailableCommandsReceived()
    {
        return ConsoleCommand.CreateRequestAvailableCommandsResponse(_availableCommands);
    }

    private void _OnHomeClickButton(object sender, RoutedEventArgs args)
    {
        _viewContainer.Content = _commandView;
    }

    private void _OnGraphViewClickButton(object sender, RoutedEventArgs args)
    {
        _viewContainer.Content = _macroEditView;
    }

    private void _OnConfigurationViewClickButton(object sender, RoutedEventArgs args)
    {
        _viewContainer.Content = _configurationView;
    }
}