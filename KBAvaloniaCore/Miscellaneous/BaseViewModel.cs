using ReactiveUI;

namespace KBAvaloniaCore.Miscellaneous;

public abstract class BaseViewModel : ReactiveObject
{
    private readonly BusyOperation _busyOperation;

    public BaseViewModel()
    {
        _busyOperation = new BusyOperation(this, nameof(BaseViewModel.IsBusy));
    }


    public bool IsBusy
    {
        get { return _busyOperation.IsBusy; }
    }

    public IDisposable StartBusyOperation()
    {
        _busyOperation.StartOperation();
        this.RaisePropertyChanged(_busyOperation.BusyProperty);
        return _busyOperation;
    }

    protected class BusyOperation : IDisposable
    {
        private ushort _busyCount;
        private readonly WeakReference<BaseViewModel> _parent;

        public BusyOperation(BaseViewModel parent, string busyProperty)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _busyCount = 0;
            _parent = new WeakReference<BaseViewModel>(parent);
            BusyProperty = busyProperty ?? throw new ArgumentNullException(nameof(busyProperty));
        }
        
        public string BusyProperty { get; }
        
        public bool IsBusy
        {
            get { return _busyCount > 0; }
        }

        public void Dispose()
        {
            --_busyCount;
            if(_parent.TryGetTarget(out var parent))
                parent.RaisePropertyChanged(this.BusyProperty);
        }

        public void StartOperation()
        {
            ++_busyCount;
        }
    }
}