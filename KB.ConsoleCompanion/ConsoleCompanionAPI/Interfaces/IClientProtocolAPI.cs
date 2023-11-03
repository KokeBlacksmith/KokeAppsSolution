using ConsoleCompanionAPI.Data;

namespace ConsoleCompanionAPI.Interfaces;

public interface IClientProtocolAPI
{
    /// <summary>
    /// Sends a command to the server
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <returns>Response from the sent command</returns>
    Task<ConsoleCommand> SendCommand(ConsoleCommand command);
    /// <summary>
    /// Request to the server the available commands to be sent
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    Task<IEnumerable<ConsoleCommand>> RequestAvailableCommands();
}
