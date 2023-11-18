using ConsoleCompanionAPI.Data;
using KB.SharpCore.Serialization;
using KB.SharpCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCompanionAPI.Protocols
{
    internal abstract class BaseTCPProtocol
    {
        protected void m_SendCommand(NetworkStream stream, ConsoleCommand command)
        {
            Result<string> messageResult = XmlSerializableHelper.SaveToXMLString(command);
            if (messageResult.IsFailure)
            {
                throw new InvalidOperationException(messageResult.MessagesAsString);
            }

            // Send command
            byte[] messageBuffer = Encoding.UTF8.GetBytes(messageResult.Value!);
            stream.Write(messageBuffer, 0, messageBuffer.Length);
        }

        protected ConsoleCommand m_ReceiveResponse(NetworkStream stream)
        {
            byte[] responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            if (String.IsNullOrEmpty(response))
            {
                throw new Exception("Server response is empty.");
            }

            Result<ConsoleCommand> responseCommandResult = XmlSerializableHelper.LoadFromXMLString<ConsoleCommand>(response);
            if (responseCommandResult.IsFailure)
            {
                throw new InvalidOperationException(responseCommandResult.MessagesAsString);
            }

            return responseCommandResult.Value!;
        }

        protected void m_AssertConnectionEndPoint(string ip, string port)
        {
            if (!RegexHelper.Network.IsIPAddress(ip))
            {
                throw new ArgumentException("IP address is not valid", nameof(ip));
            }

            if (!RegexHelper.Network.IsPort(port))
            {
                throw new ArgumentException("Port is not valid", nameof(port));
            }
        }
    }
}
