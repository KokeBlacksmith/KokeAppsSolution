namespace KB.SharpCore.Synchronization;

public class EntryCounter : IDisposable
{

    private readonly Action _onEnter;
    private readonly Action _onExit;
    protected uint _entryCount;

    public EntryCounter(Action onEnter, Action onExit)
    {
        _entryCount = 0;
        _onEnter = onEnter;
        _onExit = onExit;
    }

    public bool HasEnters
    {
        get { return _entryCount > 0; }
    }
    
    public IDisposable StartOperation()
    {
        ++_entryCount;
        _onEnter?.Invoke();

        return this;
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        --_entryCount;
        _onExit?.Invoke();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}