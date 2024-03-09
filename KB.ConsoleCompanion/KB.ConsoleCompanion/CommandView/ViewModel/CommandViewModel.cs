using Avalonia.Collections;
using Avalonia.Controls;
using ConsoleCompanionAPI.Data;
using KB.AvaloniaCore.ReactiveUI;
using KB.ConsoleCompanion.Communication;

namespace KB.ConsoleCompanion.CommandView;

internal sealed class CommandViewModel : BaseViewModel
{
    private AvaloniaList<ConsoleCommand> _commandsCollection;
    private AvaloniaList<ConsoleCommand> _userCommandsHistoryCollection;
    private GenericCommand<string?> _addCommandLineCommand;
    private VoidCommand _clearCommandCollectionCommand;
    private ConsoleCommand[]? _availableCommands;
 
    public CommandViewModel()
    {
        _commandsCollection = new AvaloniaList<ConsoleCommand>();
        _userCommandsHistoryCollection = new AvaloniaList<ConsoleCommand>();
        _addCommandLineCommand = new GenericCommand<string?>(_OnUserCommandExecuted, null);
        _clearCommandCollectionCommand = new VoidCommand(_OnClearCommandCollectionExecuted, null);

        if (!Design.IsDesignMode)
        {
            _RequestAvailableCommands();
        }
    }

    public AvaloniaList<ConsoleCommand> CommandsCollection
    {
        get { return _commandsCollection; }
        set { m_SetProperty(ref _commandsCollection, value); }
    }

    public AvaloniaList<ConsoleCommand> UserCommandsHistoryCollection
    {
        get { return _userCommandsHistoryCollection; }
        set { m_SetProperty(ref _userCommandsHistoryCollection, value); }
    }

    public GenericCommand<string?> AddCommandLineCommand
    {
        get { return _addCommandLineCommand; }
    }

    public VoidCommand ClearCommandCollectionCommand
    {
        get { return _clearCommandCollectionCommand; }
    }

    public ConsoleCommand[]? AvailableCommands
    {
        get { return _availableCommands; }
        set { m_SetProperty(ref _availableCommands, value); }
    }

    private async void _OnUserCommandExecuted(string? parameter)
    {
        if (String.IsNullOrEmpty(parameter))
        {
            return;
        }

        if(IsBusy)
        {
            return;
        }

        using (m_ExecuteBusyOperation())
        {
            ConsoleCommand userCommand = ConsoleCommand.CreateUserInput(parameter!);
        
            // Add command to collection and to view
            CommandsCollection.Add(userCommand);
            UserCommandsHistoryCollection.Add(userCommand);

            // Send Command to desired application
            Task<ConsoleCommand> responseTask = ProtocolClientController.Instance.ClientProtocolAPI.SendCommand(userCommand);
            Task _ = responseTask.ContinueWith((t) => { }, TaskContinuationOptions.OnlyOnFaulted);
            await responseTask;
            ConsoleCommand response;
            if (!responseTask.IsFaulted)
            {
                response = responseTask.Result;
            }
            else
            {
                response = ConsoleCommand.CreateResponseError(userCommand, responseTask.Exception!.Message);
            }

            CommandsCollection.Add(response);
        }
    }

    private void _OnClearCommandCollectionExecuted()
    {
        CommandsCollection.Clear();
    }

    private async void _RequestAvailableCommands()
    {
        IEnumerable<ConsoleCommand> availableCommands = await ProtocolClientController.Instance.ClientProtocolAPI.RequestAvailableCommands();
        AvailableCommands = availableCommands.ToArray();
    }
}