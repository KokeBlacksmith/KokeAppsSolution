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
    }

    public ConsoleCommand(string command, ECommandType type)
    {
        Id = Guid.NewGuid();
        Command = command;
        Type = type;
        DependencyCommandId = Guid.Empty;
    }

    public Guid Id { get; private set; }
    public string Command { get; private set; }
    public ECommandType Type { get; private set; }
    public Guid DependencyCommandId { get; private set; }


    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement();
        Id = Guid.Parse(reader.ReadElementString("Id"));
        Command = reader.ReadElementString("Command");
        Type = (ECommandType)Enum.Parse(typeof(ECommandType), reader.ReadElementString("Type"));
        DependencyCommandId = Guid.Parse(reader.ReadElementString("DependencyCommandId"));
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Id", Id.ToString());
        writer.WriteElementString("Command", Command);
        writer.WriteElementString("Type", Type.ToString());
        writer.WriteElementString("DependencyCommandId", DependencyCommandId.ToString());
    }
}
