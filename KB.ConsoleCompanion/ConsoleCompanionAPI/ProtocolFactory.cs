using ConsoleCompanionAPI.Interfaces;
using ConsoleCompanionAPI.Protocols;

namespace ConsoleCompanionAPI;

/// <summary>
/// Factory class for creating server and client instances
/// </summary>
public static class ProtocolFactory
{
    public static IServerProtocolAPI CreateServer()
    {
        return new TCPServerProtocol();
    }

    public static IClientProtocolAPI CreateClient(string ip, string port)
    {
        return new TCPClientProtocol(ip, port);
    }

}
