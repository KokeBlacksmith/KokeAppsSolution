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
}
