using KB.SharpCore.Synchronization;
using ReactiveUI;

namespace KB.AvaloniaCore.ReactiveUI;

public class ReactiveEntryCounter : EntryCounter
{
    public ReactiveEntryCounter(BaseViewModel viewModel, string propertyName)
        : base(() => viewModel.RaisePropertyChanged(propertyName), () => viewModel.RaisePropertyChanged(propertyName))
    {
        
    }
}