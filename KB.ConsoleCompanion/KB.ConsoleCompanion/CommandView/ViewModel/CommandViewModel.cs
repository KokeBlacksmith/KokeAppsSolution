using System.Collections.ObjectModel;
using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.ConsoleCompanion.CommandView;

internal sealed class CommandViewModel : BaseViewModel
{
    private IClientProtocolAPI? _client;
    private ObservableCollection<ConsoleCommand> _commandsCollection;
    private GenericCommand<string?> _addCommandLineCommand;
 
    public CommandViewModel()
    {
        _commandsCollection = new ObservableCollection<ConsoleCommand>();
        _addCommandLineCommand = new GenericCommand<string?>(_OnUserCommandExecuted, null);

        _client = ProtocolFactory.CreateClient("127.0.0.1", "55555");
    }

    public ObservableCollection<ConsoleCommand> CommandsCollection
    {
        get { return _commandsCollection; }
        set { m_SetProperty(ref _commandsCollection, value); }
    }

    public GenericCommand<string?> AddCommandLineCommand
    {
        get { return _addCommandLineCommand; }
    }

    private async void _OnUserCommandExecuted(string? parameter)
    {
        if (String.IsNullOrEmpty(parameter))
        {
            return;
        }

        if(_client == null)
        {
            ConsoleCommand errorCommand = new ConsoleCommand("Not connected to server", ConsoleCommand.ECommandType.Error);
            CommandsCollection.Add(errorCommand);
            return;
        }

        ConsoleCommand newCommand = new ConsoleCommand(parameter!, ConsoleCommand.ECommandType.UserInput);
        
        // Add command to collection and to view
        CommandsCollection.Add(newCommand);

        // Send Command to desired application
        Task<ConsoleCommand> responseTask = _client.SendCommand(newCommand);
        Task _ = responseTask.ContinueWith((t) => { }, TaskContinuationOptions.OnlyOnFaulted);
        await responseTask;
        ConsoleCommand response;
        if (!responseTask.IsFaulted)
        {
            response = responseTask.Result;
        }
        else
        {
            response = new ConsoleCommand(responseTask.Exception!.Message, ConsoleCommand.ECommandType.Error);
        }

        CommandsCollection.Add(response);
    }
}