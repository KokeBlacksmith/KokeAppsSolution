namespace KB.SharpCore.Synchronization;

public class BooleanRAIIOperation : RAIIOperation<bool>
{
    public BooleanRAIIOperation() : base(false)
    {
    }


    public override IDisposable Execute()
    {
        m_resource = true;
        return base.Execute();
    }
    public override bool CanExecute()
    {
        return !m_resource;
    }

    public override void Dispose()
    {
        m_resource = false;
    }
}
