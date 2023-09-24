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
}