using System.Runtime.CompilerServices;
using KB.AvaloniaCore.Synchronization;
using KB.SharpCore.Synchronization;
using ReactiveUI;

namespace KB.AvaloniaCore.ReactiveUI;

public abstract class BaseViewModel : ReactiveObject
{
    private readonly BooleanRAIIOperation _isBusy;

    protected BaseViewModel()
    {
        _isBusy = new ReactiveBooleanRAIIOperation(this, nameof(BaseViewModel.IsBusy));
    }

    public bool IsBusy
    {
        get { return !_isBusy.CanExecute(); }
    }

    protected IDisposable m_ExecuteBusyOperation()
    {
        return _isBusy.Execute();
    }
    
    protected void m_SetProperty<T>(ref T store, T value, [CallerMemberName] string propertyName = null)
    {
        this.RaiseAndSetIfChanged(ref store, value, propertyName);
    }
}