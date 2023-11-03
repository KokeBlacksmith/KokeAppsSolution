using ConsoleCompanionAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCompanionAPI.Interfaces;
public interface IServerProtocolAPI
{
    event Func<ConsoleCommand, ConsoleCommand>? OnCommandReceived;
    event Func<ConsoleCommand>? OnRequestAvailableCommandsReceived;

    bool IsConnected { get; }

    void Start(string ip, string port);
    void Stop();
    Task StopAsync();
}
