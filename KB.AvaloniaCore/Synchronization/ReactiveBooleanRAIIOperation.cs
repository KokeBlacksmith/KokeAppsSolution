using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.Synchronization;
using ReactiveUI;

namespace KB.AvaloniaCore.Synchronization;

public class ReactiveBooleanRAIIOperation : BooleanRAIIOperation
{
    private readonly BaseViewModel _parentViewModel;
    private readonly string _propertyName;
    public ReactiveBooleanRAIIOperation(BaseViewModel viewModel, string propertyName, bool defaultValue = false)
    {
        _parentViewModel = viewModel;
        _propertyName = propertyName;
    }

    public override IDisposable Execute()
    {
        var result = base.Execute();
        _parentViewModel.RaisePropertyChanged(_propertyName);
        return result;
    }

    public override bool CanExecute()
    {
        return base.CanExecute();
    }

    protected override void m_ReleaseResource()
    {
        base.m_ReleaseResource();
        _parentViewModel.RaisePropertyChanged(_propertyName);
    }
}
