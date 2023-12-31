﻿using Avalonia.Collections;
using Avalonia.Controls;
using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.ConsoleCompanion.CommandView;

internal sealed class CommandViewModel : BaseViewModel
{
    private IClientProtocolAPI? _client;
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
            _client = ProtocolFactory.CreateClient("127.0.0.1", "55555");
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
            if (_client == null)
            {
                ConsoleCommand errorCommand = new ConsoleCommand("Not connected to server", ConsoleCommand.ECommandType.Error);
                CommandsCollection.Add(errorCommand);
                return;
            }

            ConsoleCommand userCommand = ConsoleCommand.CreateUserInput(parameter!);
        
            // Add command to collection and to view
            CommandsCollection.Add(userCommand);
            UserCommandsHistoryCollection.Add(userCommand);

            // Send Command to desired application
            Task<ConsoleCommand> responseTask = _client.SendCommand(userCommand);
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
        IEnumerable<ConsoleCommand> availableCommands = await _client!.RequestAvailableCommands();
        AvailableCommands = availableCommands.ToArray();
    }
}