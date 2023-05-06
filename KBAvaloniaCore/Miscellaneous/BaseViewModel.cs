using System.Runtime.CompilerServices;
using ReactiveUI;

namespace KBAvaloniaCore.Miscellaneous;

public abstract class BaseViewModel : ReactiveObject
{
    private class BusyOperation : IDisposable
    {
        private UInt16 _busyCount;
        public void Dispose() { }
    }
    
    private bool _isBusy;
    
    public bool IsBusy
    {
        get
        {
            return _isBusy;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
    }
}