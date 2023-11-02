using ConsoleCompanionAPI.Data;
using KB.SharpCore.Extensions;
using KB.SharpCore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCompanionAPI.Protocols
{
    internal class BaseTCPProtocol
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
    }
}
