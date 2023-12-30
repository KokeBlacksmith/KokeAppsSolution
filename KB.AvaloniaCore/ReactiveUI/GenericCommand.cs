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
        if (_IsAssignable(parameter))
        {
            if(parameter == null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                return CanExecute(default(T));
#pragma warning restore CS8604 // Possible null reference argument.
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
        if (_IsAssignable(parameter))
        {
            if (parameter == null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                Execute(default(T));
#pragma warning restore CS8604 // Possible null reference argument.
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

    private bool _IsAssignable(object? parameter)
    {
        if (parameter == null)
        {
            // Handle null case. 
            // Assuming T is a reference type, null is assignable.
            // If T is a value type, null is not assignable.
            return !typeof(T).IsValueType;
        }
        else
        {
            return typeof(T).IsAssignableFrom(parameter.GetType());
        }
    }
}
