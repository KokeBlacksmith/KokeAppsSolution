using ConsoleCompanionAPI.Data;
using ConsoleCompanionAPI.Interfaces;
using KB.SharpCore.Serialization;
using KB.SharpCore.Utils;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;

namespace ConsoleCompanionAPI.Protocols;

internal class TCPServerProtocol : BaseTCPProtocol, IServerProtocolAPI
{
    private bool _isDisposed;
    public event Func<ConsoleCommand, ConsoleCommand>? OnCommandReceived;
    public event Func<ConsoleCommand>? OnRequestAvailableCommandsReceived;

    private TcpListener? _listener;
    private Task? _listenerTask;
    private CancellationTokenSource? _listenerTaskCancellationTokenSource;


    public bool IsConnected
    {
        get => _listener != null && _listener.Server.IsBound;
    }

    [SecurityCritical]
    public void Start(string ip, string port)
    {
        m_AssertConnectionEndPoint(ip, port);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port));
        _listener = new TcpListener(endPoint);
        _listener.Start();
        _listenerTaskCancellationTokenSource = new CancellationTokenSource();
        _listenerTask = _ListenerWorker(_listenerTaskCancellationTokenSource.Token);
    }

    public void Stop()
    {
        StopAsync().Wait();
    }

    public async Task StopAsync()
    {
        if (IsConnected)
        {
            _listener?.Stop();
            _listener = null;
            _listenerTaskCancellationTokenSource?.Cancel();
            await _listenerTask!;
            _listenerTask?.Dispose();
            _listenerTask = null;
            _listenerTaskCancellationTokenSource?.Dispose();
            _listenerTaskCancellationTokenSource = null;
        }
    }

    private Task _ListenerWorker(CancellationToken token)
    {
        try
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    TcpClient client = await _listener!.AcceptTcpClientAsync();
                    // Create a new thread to handle the client
                    Thread clientThread = new Thread(() => _HandleClient(client));
                    clientThread.Start();
                }
            }, token);
        }
        catch (OperationCanceledException)
        {
            return Task.CompletedTask;
        }
    }

    private async void _HandleClient(TcpClient client)
    {
        try
        {
            await using NetworkStream stream = client.GetStream();
            ConsoleCommand clientCommand = m_ReceiveResponse(stream);

            // Process message
            ConsoleCommand responseToClient;

            if(clientCommand.Id == ConsoleCommand.REQUEST_AVAILABLE_COMMANDS_ID)
            {
                if (OnRequestAvailableCommandsReceived == null)
                {
                    responseToClient = ConsoleCommand.CreateResponseError(clientCommand, "Server received command but is not listening to commands");
                }
                else
                {
                    responseToClient = OnRequestAvailableCommandsReceived!.Invoke();
                }
            }
            else if (OnCommandReceived == null)
            {
                responseToClient = ConsoleCommand.CreateResponseError(clientCommand, "Server received command but is not listening to commands");
            }
            else
            {
                responseToClient = OnCommandReceived!.Invoke(clientCommand);
            }

            // Respond to client
            m_SendCommand(stream, responseToClient);
        }
        finally
        {
            client.Close();
        }

    }

    #region IDisposable Pattern

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Release managed resources (if any)
                Stop();
            }

            // Release unmanaged resources (e.g., file handles, network connections)
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    ~TCPServerProtocol()
    {
        Dispose(false);
    }

    #endregion
}
