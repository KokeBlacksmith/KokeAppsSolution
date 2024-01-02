using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.DataAnnotations;
using KB.SharpCore.Utils;
using System.ComponentModel.DataAnnotations;

namespace KB.ConsoleCompanion.ConfigurationView.ViewModel;

internal class ConfigurationViewModel : BaseViewModel
{
    private readonly VoidCommand _applyCommand;
    private readonly VoidCommand _cancelCommand;
    private readonly GenericCommand<string?> _connectCommand;
    private string _ipAddress;
    private string _portNumber;
    private bool _isConnected;
    private KB.SharpCore.IO.Path _storagePath;

    public ConfigurationViewModel()
    {
        // Localhost is the default IP address
        _ipAddress = System.Net.IPAddress.Loopback.ToString();
        _portNumber = "5555";
        _storagePath = new KB.SharpCore.IO.Path(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "KB_ConsoleCompanion", System.IO.Path.DirectorySeparatorChar.ToString()));
        _applyCommand = new VoidCommand(_OnApplyCommandExecute, null);
        _cancelCommand = new VoidCommand(_OnCancelCommandExecute, null);
        _connectCommand = new GenericCommand<string?>(_OnConnectCommandExecute, _OnConnectCommandCanExecute);
    }

    public VoidCommand ApplyCommand => _applyCommand;
    public VoidCommand CancelCommand => _cancelCommand;
    public GenericCommand<string?> ConnectCommand => _connectCommand;

    [Required]
    [IPAddress(isIPv4: true, isIPv6: false)]

    public string IPAddress
    {
        get { return _ipAddress; }
        set { m_SetProperty(ref _ipAddress, value); }
    }

    [Required]
    [PortNumber]
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
        //return RegexHelper.Network.IsIPAddress(ipAddress!);
        return !String.IsNullOrWhiteSpace(ipAddress) && RegexHelper.Network.IsIPv4(ipAddress!);
    }
}
