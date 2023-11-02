using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Interfaces;

namespace KB.ConsoleCompanion;

public partial class MainConsoleCompanion : UserControl
{
    IServerProtocolAPI _server;
    public MainConsoleCompanion()
    {
        //Test
        _server = ProtocolFactory.CreateServer();
        _server.Start("127.0.0.1", "55555");
        // End Test
        InitializeComponent();
    }
}