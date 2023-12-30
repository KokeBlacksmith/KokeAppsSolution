using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.Utils;

namespace KB.ConsoleCompanion.ConfigurationView.ViewModel;

internal class ConfigurationViewModel : BaseViewModel
{
    private readonly EmptyCommand _applyCommand;
    private readonly EmptyCommand _cancelCommand;
    private readonly GenericCommand<string?> _connectCommand;
    private string _ipAddress;
    private string _portNumber;
    private bool _isConnected;
    private KB.SharpCore.IO.Path _storagePath;

    public ConfigurationViewModel()
    {
        _ipAddress = System.Net.IPAddress.Loopback.ToString();
        _storagePath = new KB.SharpCore.IO.Path(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "KB_ConsoleCompanion"));
        _applyCommand = new EmptyCommand(_OnApplyCommandExecute, null);
        _cancelCommand = new EmptyCommand(_OnCancelCommandExecute, null);
        _connectCommand = new GenericCommand<string?>(_OnConnectCommandExecute, _OnConnectCommandCanExecute);
    }

    public EmptyCommand ApplyCommand => _applyCommand;
    public EmptyCommand CancelCommand => _cancelCommand;
    public GenericCommand<string?> ConnectCommand => _connectCommand;


    public string IPAddress
    {
        get { return _ipAddress; }
        set { m_SetProperty(ref _ipAddress, value); }
    }

    public string PortNumber
    {
        get { return _portNumber; }
        set { m_SetProperty(ref _portNumber, value); }
    }

    public bool IsConnected
    {
        get { return _isConnected; }
        set { m_SetProperty(ref _isConnected, value); }
    }

    public KB.SharpCore.IO.Path StorageDirectoryPath
    {
        get { return _storagePath; }
        set { m_SetProperty(ref _storagePath, value); }
    }

    private void _OnApplyCommandExecute()
    {
        //TODO: save the configuration to the model
    }

    private void _OnCancelCommandExecute()
    {
        // TODO: Get values from model
    }

    private void _OnConnectCommandExecute(string? ipAddress)
    {

    }

    private bool _OnConnectCommandCanExecute(string? ipAddress)
    {
        if(String.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }

        return RegexHelper.Network.IsIPAddress(ipAddress!);
    }
}
