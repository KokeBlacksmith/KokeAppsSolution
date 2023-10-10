using System.Runtime.CompilerServices;
using KB.SharpCore.Synchronization;
using ReactiveUI;

namespace KB.AvaloniaCore.ReactiveUI;

public abstract class BaseViewModel : ReactiveObject
{
    protected readonly ReactiveEntryCounter m_busyOperation;

    protected BaseViewModel()
    {
        m_busyOperation = new ReactiveEntryCounter(this, nameof(BaseViewModel.IsBusy));
    }

    public bool IsBusy
    {
        get { return m_busyOperation.HasEnters; }
    }
    
    protected void m_SetProperty<T>(ref T store, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(store, value))
        {
            return;
        }

        store = value;
        this.RaisePropertyChanged(propertyName);
    }
}