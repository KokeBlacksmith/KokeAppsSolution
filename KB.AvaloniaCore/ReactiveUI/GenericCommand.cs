using System.Windows.Input;

namespace KB.AvaloniaCore.ReactiveUI;
public sealed class GenericCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public GenericCommand(Action<T> execute, Func<T, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(T parameter)
    {
        if(_canExecute != null)
        {
            return _canExecute(parameter);
        }
        else
        {
            return true;
        }
    }

    public void Execute(T parameter)
    {
        _execute(parameter);
    }

    bool ICommand.CanExecute(object? parameter)
    {
        if(typeof(T).IsAssignableFrom(parameter?.GetType() ?? null))
        {
            if(parameter == null)
            {
                return CanExecute(default(T));
            }
            else
            {
                return CanExecute(((T)parameter)!);
            }
        }
        else
        {
            throw new ArgumentException($"Parameter must be of type {typeof(T)}");
        }
    }

    void ICommand.Execute(object? parameter)
    {
        if (typeof(T).IsAssignableFrom(parameter?.GetType() ?? null))
        {
            if (parameter == null)
            {
                Execute(default(T));
            }
            else
            {
                Execute(((T)parameter)!);
            }
        }
        else
        {
            throw new ArgumentException($"Parameter must be of type {typeof(T)}");
        }
    }
}
