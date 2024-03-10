using Avalonia.Collections;
using Avalonia.Controls;
using ConsoleCompanionAPI.Data;
using KB.AvaloniaCore.ReactiveUI;
using KB.ConsoleCompanion.Communication;
using KB.ConsoleCompanion.DataModels;

namespace KB.ConsoleCompanion.MacroEditView;

internal class MacroEditViewModel : BaseViewModel
{
    private readonly AvaloniaList<ConsoleCommandViewModel> _availableCommands;
    private readonly AvaloniaList<ConsoleCommandViewModel> _macroCommands;

    private readonly GenericCommand<ConsoleCommandViewModel> _sortUpMacroItemCommand;
    private readonly GenericCommand<ConsoleCommandViewModel> _sortDownMacroItemCommand;
    private readonly GenericCommand<(int, int)> _swapMacroItemsCommand;

    public MacroEditViewModel()
    {
        _availableCommands = new AvaloniaList<ConsoleCommandViewModel>();
        _macroCommands = new AvaloniaList<ConsoleCommandViewModel>();

        _sortUpMacroItemCommand = new GenericCommand<ConsoleCommandViewModel>(SortUpMacroItemCommandExecute, _CanSortUpMacroItemCommandExecute);
        _sortDownMacroItemCommand = new GenericCommand<ConsoleCommandViewModel>(SortDownMacroItemCommandExecute, _CanSortDownMacroItemCommandExecute);
        _swapMacroItemsCommand = new GenericCommand<(int, int)>(_OnSwapMacroItemsCommandExecuted, null);

        if (!Design.IsDesignMode)
        {
            _RequestAvailableCommands();
        }
    }

    private void _OnSwapMacroItemsCommandExecuted((int, int) tuple)
    {
        MacroCommands.Move(tuple.Item1, tuple.Item2);
    }

    public GenericCommand<(int, int)> SwapMacroItemsCommand
    {
        get { return _swapMacroItemsCommand; }
    }

    public GenericCommand<ConsoleCommandViewModel> SortUpMacroItemCommand
    {
        get { return _sortUpMacroItemCommand; }
    }

    public GenericCommand<ConsoleCommandViewModel> SortDownMacroItemCommand
    {
        get { return _sortDownMacroItemCommand; }
    }

    public AvaloniaList<ConsoleCommandViewModel> AvailableCommands
    {
        get { return _availableCommands; }
    }

    public AvaloniaList<ConsoleCommandViewModel> MacroCommands
    {
        get { return _macroCommands; }
    }

    public void ClearMacro()
    {
        _macroCommands.Clear();
    }

    public void SaveMacro()
    { }

    public void LoadMacro()
    { }

    public void DeleteMacro()
    { }

    public void ExecuteMacro()
    { }

    private void SortUpMacroItemCommandExecute(ConsoleCommandViewModel command)
    {
        int index = _macroCommands.IndexOf(command);
        _macroCommands.Move(index, index - 1);
    }

    private bool _CanSortUpMacroItemCommandExecute(ConsoleCommandViewModel command)
    {
        return _macroCommands.IndexOf(command) > 0;
    }

    private void SortDownMacroItemCommandExecute(ConsoleCommandViewModel command)
    {
        int index = _macroCommands.IndexOf(command);
        _macroCommands.Move(index, index + 1);
    }

    private bool _CanSortDownMacroItemCommandExecute(ConsoleCommandViewModel command)
    {
        return _macroCommands.IndexOf(command) < _macroCommands.Count - 1;
    }

    private async void _RequestAvailableCommands()
    {
        AvailableCommands.Clear();
        IEnumerable<ConsoleCommand> availableCommands = await ProtocolClientController.Instance.ClientProtocolAPI.RequestAvailableCommands();
        AvailableCommands.AddRange(availableCommands.Select(command => new ConsoleCommandViewModel(command)));

        // To test visual representation
        MacroCommands.AddRange(AvailableCommands);
    }
}
