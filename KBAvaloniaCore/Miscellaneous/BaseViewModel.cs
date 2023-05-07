using ReactiveUI;

namespace KBAvaloniaCore.Miscellaneous;

public abstract class BaseViewModel : ReactiveObject
{
    private readonly BusyOperation _busyOperation;

    public BaseViewModel()
    {
        _busyOperation = new BusyOperation();
        _busyOperation.Disposed = _OnBusyOperationDisposed;
    }


    public bool IsBusy
    {
        get { return _busyOperation.IsBusy; }
    }

    public IDisposable StartBusyOperation()
    {
        _busyOperation.StartOperation();
        this.RaisePropertyChanged(nameof(BaseViewModel.IsBusy));
        return _busyOperation;
    }

    private void _OnBusyOperationDisposed()
    {
        this.RaisePropertyChanged(nameof(BaseViewModel.IsBusy));
    }

    private class BusyOperation : IDisposable
    {
        private ushort _busyCount;
        public Action Disposed;

        public bool IsBusy
        {
            get { return _busyCount > 0; }
        }

        public void Dispose()
        {
            --_busyCount;
            Disposed?.Invoke();
        }

        public void StartOperation()
        {
            ++_busyCount;
        }
    }
}