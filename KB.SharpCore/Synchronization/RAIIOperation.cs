namespace KB.SharpCore.Synchronization;

/// <summary>
/// resource acquisition is initialization
/// Acquires a resource (like a lock) in the constructor and releases it in the destructor.
/// </summary>
public abstract class RAIIOperation<T> : IDisposable
{
    protected T m_resource;

    protected RAIIOperation(T resource)
    {
        m_resource = resource;
    }

    public T GetResource()
    {
        return m_resource;
    }

    public virtual IDisposable Execute()
    {
        return this;
    }

    public abstract bool CanExecute();

    public abstract void Dispose();
}
