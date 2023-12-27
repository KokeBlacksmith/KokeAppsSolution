using System.ComponentModel;

namespace KB.SharpCore.Synchronization;

/// <summary>
/// resource acquisition is initialization
/// Acquires a resource (like a lock) in the constructor and releases it in the destructor.
/// </summary>
public abstract class RAIIOperation<T>
{
    private class VoidDisposable : IDisposable
    {
        private readonly Action _onDisposeAction;

        public VoidDisposable(Action onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
        }

        public void Dispose()
        {
            _onDisposeAction.Invoke();
        }
    }

    public readonly IDisposable _voidDisposable;

    protected T m_resource;

    protected RAIIOperation(T resource)
    {
        _voidDisposable = new VoidDisposable(m_ReleaseResource);
        m_resource = resource;
    }

    public T GetResource()
    {
        return m_resource;
    }

    public virtual IDisposable Execute()
    {
        return _voidDisposable;
    }

    public abstract bool CanExecute();

    protected abstract void m_ReleaseResource();
}
