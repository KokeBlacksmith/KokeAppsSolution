using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace ConsoleCompanionAPI.Protocols;
internal class TCPClientProtocol : BaseTCPProtocol, IClientProtocolAPI
{
    private readonly IPEndPoint _endPoint;

    public TCPClientProtocol(string ip, string port)
    {
        s_AssertConnectionEndPoint(ip, port);

        _endPoint = new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port));
    }

    public event Action<ConsoleCommand>? OnCommandReceived;


    public Task<ConsoleCommand> SendCommand(ConsoleCommand command)
    {
        return Task.Run(async () => { 
            using TcpClient client = new TcpClient();
            client.Connect(_endPoint);

            if (!client.Connected)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            await using NetworkStream stream = client.GetStream();

            s_SendCommand(stream, command);
            return s_ReceiveResponse(stream);
        });
    }

    public async Task<IEnumerable<ConsoleCommand>> RequestAvailableCommands()
    {
        ConsoleCommand response = await SendCommand(ConsoleCommand.CreateRequestAvailableCommands());
        return ConsoleCommand.ParseAvailableCommandsResponse(response);
    }
}
