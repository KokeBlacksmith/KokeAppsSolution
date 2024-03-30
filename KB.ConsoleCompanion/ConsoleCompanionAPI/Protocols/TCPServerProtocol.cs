using KB.ConsoleCompanionAPI.Data;
using KB.ConsoleCompanionAPI.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace KB.ConsoleCompanionAPI.Protocols;

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
        s_AssertConnectionEndPoint(ip, port);

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
            if(_listenerTask != null)
            {
                // Wait for the listener task to finish (if it hasn't already)
                await _listenerTask!;
                _listenerTask.Dispose();
                _listenerTask = null;
                _listenerTaskCancellationTokenSource?.Dispose();
                _listenerTaskCancellationTokenSource = null;
            }
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in listener worker: {ex.Message}");
            return Task.FromException(ex);
        }
    }

    private async void _HandleClient(TcpClient client)
    {
        try
        {
            await using NetworkStream stream = client.GetStream();
            ConsoleCommand clientCommand = s_ReceiveResponse(stream);

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
            s_SendCommand(stream, responseToClient);
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
