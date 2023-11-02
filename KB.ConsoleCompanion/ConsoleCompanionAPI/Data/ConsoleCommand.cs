using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleCompanionAPI.Data;
public class ConsoleCommand : IXmlSerializable
{
    public enum ECommandType : ushort
    {
        Info,
        UserInput,
        Warning,
        Error,
    }

    private ConsoleCommand()
    {
        Id = Guid.Empty;
        Command = String.Empty;
        Type = ECommandType.Info;
        DependencyCommandId = Guid.Empty;
        Time = DateTime.Now;
    }

    public ConsoleCommand(string command, ECommandType type)
    {
        Id = Guid.NewGuid();
        Command = command;
        Type = type;
        DependencyCommandId = Guid.Empty;
        Time = DateTime.Now;
    }

    public Guid Id { get; private set; }
    public string Command { get; private set; }
    public ECommandType Type { get; private set; }
    public Guid DependencyCommandId { get; private set; }
    public DateTime Time { get; private set; }


    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement();
        Id = Guid.Parse(reader.ReadElementString(nameof(Id)));
        Command = reader.ReadElementString(nameof(Command));
        Type = (ECommandType)Enum.Parse(typeof(ECommandType), reader.ReadElementString(nameof(Type)));
        DependencyCommandId = Guid.Parse(reader.ReadElementString(nameof(DependencyCommandId)));
        Time = DateTime.Parse(reader.ReadElementString(nameof(Time)));
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString(nameof(Id), Id.ToString());
        writer.WriteElementString(nameof(Command), Command);
        writer.WriteElementString(nameof(Type), Type.ToString());
        writer.WriteElementString(nameof(DependencyCommandId), DependencyCommandId.ToString());
        writer.WriteElementString(nameof(Time), Time.ToString());
    }
}
