using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;

namespace KB.ConsoleCompanion;

public partial class MainConsoleCompanion : UserControl
{
    IServerProtocolAPI _server;
    public MainConsoleCompanion()
    {
        //Test
        _InitServer();
        // End Test

        InitializeComponent();
    }

    private void _InitServer()
    {
        _server = ProtocolFactory.CreateServer();
        _server.Start("127.0.0.1", "55555");

        _server.OnCommandReceived += (command) =>
        {
            ConsoleCommand response = new ConsoleCommand($"Response from command '{command.Command}' received sucessfully!", ConsoleCommand.ECommandType.Info);
            return response;
        };
    }
}