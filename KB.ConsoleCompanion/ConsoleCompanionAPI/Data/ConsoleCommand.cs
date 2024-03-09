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

    public static readonly Guid REQUEST_AVAILABLE_COMMANDS_ID = new Guid("550e8400-e29b-41d4-a716-446655440000");
    public static readonly Guid REQUEST_AVAILABLE_COMMANDS_RESPONSE_ID = new Guid("6b29fc40-3949-4d38-9b34-065f8a7d47f7");

    private const string c_AVAILABLE_COMMANDS_SEPARATOR = ";;";

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

    #region Static Methods

    public static ConsoleCommand CreateRequestAvailableCommands()
    {
        return new ConsoleCommand("Request Available Commands", ECommandType.UserInput) { Id = REQUEST_AVAILABLE_COMMANDS_ID };
    }

    public static ConsoleCommand CreateRequestAvailableCommandsResponse(IEnumerable<string> commands)
    {
        return new ConsoleCommand(String.Join(c_AVAILABLE_COMMANDS_SEPARATOR, commands), ECommandType.Info)
        {
            Id = REQUEST_AVAILABLE_COMMANDS_RESPONSE_ID,
            DependencyCommandId = REQUEST_AVAILABLE_COMMANDS_RESPONSE_ID,
        };
    }

    public static IEnumerable<ConsoleCommand> ParseAvailableCommandsResponse(ConsoleCommand serverCommand)
    {
        if(serverCommand.Id != REQUEST_AVAILABLE_COMMANDS_RESPONSE_ID)
        {
            throw new InvalidOperationException("Cannot parse a command that is not a response to a request for available commands");
        }

        foreach (string commandMessage in serverCommand.Command.Split(c_AVAILABLE_COMMANDS_SEPARATOR))
        {
            yield return new ConsoleCommand(commandMessage, ECommandType.Info);
        }
    }

    public static ConsoleCommand CreateResponseInfo(ConsoleCommand? userCommand, string message)
    {
        return ConsoleCommand._CreateResponse(userCommand, message, ECommandType.Info);
    }

    public static ConsoleCommand CreateResponseWarning(ConsoleCommand? userCommand, string message)
    {
        return ConsoleCommand._CreateResponse(userCommand, message, ECommandType.Warning);
    }

    public static ConsoleCommand CreateResponseError(ConsoleCommand? userCommand, string message)
    {
        return ConsoleCommand._CreateResponse(userCommand, message, ECommandType.Error);
    }

    public static ConsoleCommand CreateUserInput(string message)
    {
        return new ConsoleCommand(message, ECommandType.UserInput);
    }

    private static ConsoleCommand _CreateResponse(ConsoleCommand? command, string message, ECommandType type)
    {
        if(type == ECommandType.UserInput)
        {
            throw new InvalidOperationException("Cannot create a response with type UserInput");
        }

        return new ConsoleCommand(message, type)
        {
            DependencyCommandId = command?.Id ?? Guid.Empty
        };
    }

    #endregion

    #region XML Serialization

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


    #endregion
}
