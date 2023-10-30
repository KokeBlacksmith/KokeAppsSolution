using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Threading;
using KB.AvaloniaCore.Controls.Log;
using KB.AvaloniaCore.ReactiveUI;
using KB.ConsoleCompanion.DataModels;
using KB.SharpCore.Utils;
using ReactiveUI;

namespace KB.ConsoleCompanion.CommandView;

internal class CommandViewModel : BaseViewModel
{
    private ObservableCollection<ConsoleCommand> _commandsCollection;
    private GenericCommand<string?> _addCommandLineCommand;
    
    public CommandViewModel()
    {
        _commandsCollection = new ObservableCollection<ConsoleCommand>();
        _addCommandLineCommand = new GenericCommand<string?>(_OnUserCommandExecuted, null);
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

    private void _OnUserCommandExecuted(string? parameter)
    {
        if (String.IsNullOrEmpty(parameter))
        {
            return;
        }

        //TODO: Check if it is a valid command
        ConsoleCommand newCommand = new ConsoleCommand(parameter!, ConsoleCommand.ECommandType.UserInput);
        CommandsCollection.Add(newCommand);
    }
}