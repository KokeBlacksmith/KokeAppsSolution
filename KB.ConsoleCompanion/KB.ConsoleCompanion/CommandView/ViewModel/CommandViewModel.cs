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
    private ICommand _addCommandLineCommand;
    
    public CommandViewModel()
    {
        _commandsCollection = new ObservableCollection<ConsoleCommand>();
        _addCommandLineCommand = ReactiveCommand.Create<ConsoleCommand>(_OnUserCommandExecuted);
    }

    public ObservableCollection<ConsoleCommand> CommandsCollection
    {
        get { return _commandsCollection; }
        set { m_SetProperty(ref _commandsCollection, value); }
    }

    public ICommand AddCommandLineCommand
    {
        get { return _addCommandLineCommand; }
    }

    private void _OnUserCommandExecuted(ConsoleCommand parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        //TODO: Check if it is a valid command

        CommandsCollection.Add(parameter);
    }
}