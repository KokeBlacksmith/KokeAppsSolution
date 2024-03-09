using ConsoleCompanionAPI.Data;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.ConsoleCompanion.DataModels;

internal class ConsoleCommandViewModel : BaseModelReplicationViewModel<ConsoleCommand>
{
    private Guid _id;
    private string _command;
    private ConsoleCommand.ECommandType _type;
    private Guid _dependencyCommandId;
    private DateTime _time;


    public ConsoleCommandViewModel(ConsoleCommand model) : base(model)
    {
    }

    [PropertyReplicationName(nameof(ConsoleCommand.Id))]
    public Guid Id
    {
        get { return _id; }
        set { m_SetProperty(ref _id, value); }
    }

    [PropertyReplicationName(nameof(ConsoleCommand.Command))]
    public string Command
    {
        get { return _command; }
        set { m_SetProperty(ref _command, value); }
    }

    [PropertyReplicationName(nameof(ConsoleCommand.Type))]
    public ConsoleCommand.ECommandType Type
    {
        get { return _type; }
        set { m_SetProperty(ref _type, value); }
    }

    [PropertyReplicationName(nameof(ConsoleCommand.DependencyCommandId))]
    public Guid DependencyCommandId
    {
        get { return _dependencyCommandId; }
        set { m_SetProperty(ref _dependencyCommandId, value); }
    }

    [PropertyReplicationName(nameof(ConsoleCommand.Time))]
    public DateTime Time
    {
        get { return _time; }
        set { m_SetProperty(ref _time, value); }
    }
}
