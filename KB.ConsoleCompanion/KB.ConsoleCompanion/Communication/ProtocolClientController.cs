using KB.ConsoleCompanionAPI;
using KB.ConsoleCompanionAPI.Interfaces;
using KB.SharpCore.DesignPatterns.Singleton;

namespace KB.ConsoleCompanion.Communication
{
    internal class ProtocolClientController : BaseSingleton<ProtocolClientController>
    {
        private IClientProtocolAPI? _client;
        private string _serverIP = "127.0.0.1";
        private string _serverPort = "55555";

        public string ServerIP
        {
            get { return _serverIP; }
            set { _serverIP = value; }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }


        private ProtocolClientController()
        {
            
        }

        public void Reconnect()
        {
            _client = ProtocolFactory.CreateClient(_serverIP, _serverPort) ?? throw new Exception($"Failed to create {nameof(IClientProtocolAPI)}");
        }

        public IClientProtocolAPI ClientProtocolAPI
        {
            get 
            { 
                if (_client == null)
                {
                    Reconnect();
                }

                return _client!; 
            }
        }
    }
}
