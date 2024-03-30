using Avalonia.Controls;
using Avalonia.Interactivity;
using KB.ConsoleCompanion.Communication;

namespace KB.ConsoleCompanion;

public partial class MainConsoleCompanion : UserControl
{
    private string[]? _args;
    private readonly CommandView.CommandView _commandView;
    private readonly MacroEditView.MacroEditView _macroEditView;
    private readonly ConfigurationView.ConfigurationView _configurationView;

    //IServerProtocolAPI? _server;
    public MainConsoleCompanion()
    {
        //Test
        //_InitServer();
        // End Test

        InitializeComponent();

        _commandView = new CommandView.CommandView();
        _macroEditView = new MacroEditView.MacroEditView();
        _configurationView = new ConfigurationView.ConfigurationView();
        _viewContainer.Content = _commandView;
    }

    public string[]? Args
    {
        get { return _args; }
        set
        {
            _args = value;
            if (_args != null)
            {
                try
                {
                    string dataPath = "";
                    for (int i = 0; i < _args.Length; i++)
                    {
                        switch (_args[i])
                        {
                            case "--port":
                                if (i + 1 < _args.Length)
                                {
                                    ProtocolClientController.Instance.ServerPort = _args[i + 1];
                                }
                                else
                                {
                                    throw new Exception("Port not specified");
                                }
                                break;
                            case "--ip":
                                if (i + 1 < _args.Length)
                                {
                                    ProtocolClientController.Instance.ServerIP = _args[i + 1];
                                }
                                else
                                {
                                    throw new Exception("IP not specified");
                                }
                                break;
                            case "--savePath":
                                if (i + 1 < _args.Length)
                                {
                                    dataPath = _args[i + 1];
                                }
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("Failed to parse arguments", ex);
                }
            }
        }
    }

    //private string[] _availableCommands = { 
    //    "Throw Error",
    //    "Throw Warning",
    //    "Throw Info",
    //    "Say Hello",
    //    "Say Goodbye",
    //    "Say How are you?",
    //}; 

    //private void _InitServer()
    //{
    //    _server = ProtocolFactory.CreateServer();
    //    _server.Start("127.0.0.1", "55555");

    //    _server.OnCommandReceived += OnCommandReceived;
    //    _server.OnRequestAvailableCommandsReceived += OnRequestAvailableCommandsReceived;
    //}

    //private ConsoleCommand OnCommandReceived(ConsoleCommand clientCommand)
    //{
    //    ConsoleCommand response = new ConsoleCommand($"Response from command '{clientCommand.Command}' received sucessfully!", ConsoleCommand.ECommandType.Info);
    //    return response;
    //}

    //private ConsoleCommand OnRequestAvailableCommandsReceived()
    //{
    //    return ConsoleCommand.CreateRequestAvailableCommandsResponse(_availableCommands);
    //}

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