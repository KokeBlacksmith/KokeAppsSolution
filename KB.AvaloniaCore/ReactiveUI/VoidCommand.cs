using System.Windows.Input;

namespace KB.AvaloniaCore.ReactiveUI
{
    public sealed class VoidCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public VoidCommand(Action execute, Func<bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute()
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            else
            {
                return true;
            }
        }

        public void Execute()
        {
            _execute();
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object? parameter)
        {
            Execute();
        }
    }
}
