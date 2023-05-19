using ReactiveUI;

namespace KBAvaloniaCore.ReactiveUI;

public abstract class BaseViewModel : ReactiveObject
{
    protected readonly BusyOperation m_busyOperation;

    public BaseViewModel()
    {
        m_busyOperation = new BusyOperation(this, nameof(BaseViewModel.IsBusy));
    }


    public bool IsBusy
    {
        get { return m_busyOperation.IsBusy; }
    }

    protected class BusyOperation : IDisposable
    {
        private readonly WeakReference<BaseViewModel> _parent;
        private ushort _busyCount;

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
            if (_parent.TryGetTarget(out BaseViewModel? parent))
                parent.RaisePropertyChanged(BusyProperty);
        }

        public IDisposable StartOperation()
        {
            ++_busyCount;
            if (_parent.TryGetTarget(out BaseViewModel? parent))
                parent.RaisePropertyChanged(BusyProperty);

            return this;
        }
    }
}