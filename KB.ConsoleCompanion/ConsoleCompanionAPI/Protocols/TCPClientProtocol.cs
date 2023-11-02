using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;
using KB.SharpCore.Extensions;
using KB.SharpCore.Serialization;
using KB.SharpCore.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleCompanionAPI.Protocols;
internal class TCPClientProtocol : IClientProtocolAPI
{
    private readonly IPEndPoint _endPoint;

    public TCPClientProtocol(string ip, string port)
    {
        if (!RegexHelper.Network.IsIPAddress(ip))
        {
            throw new ArgumentException("IP address is not valid", nameof(ip));
        }

        if (!RegexHelper.Network.IsPort(port))
        {
            throw new ArgumentException("Port is not valid", nameof(port));
        }

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

            Result<string> messageResult = XmlSerializableHelper.SaveToXMLString(command);
            if(messageResult.IsFailure)
            {
                throw new InvalidOperationException(messageResult.MessagesAsString);
            }

            // Send command
            byte[] messageBuffer = Encoding.UTF8.GetBytes(messageResult.Value!);
            stream.Write(messageBuffer, 0, messageBuffer.Length);

            // Receive response
            byte[] responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            if(String.IsNullOrEmpty(response))
            {
                throw new Exception("Server response is empty.");
            }

            Result<ConsoleCommand> responseCommandResult = XmlSerializableHelper.LoadFromXMLString<ConsoleCommand>(response);
            if(responseCommandResult.IsFailure)
            {
                throw new InvalidOperationException(responseCommandResult.MessagesAsString);
            }

            return responseCommandResult.Value!;
        });
    }
}
