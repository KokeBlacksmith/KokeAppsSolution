using ConsoleCompanionAPI;
using ConsoleCompanionAPI.Interfaces;
using KB.SharpCore.DesignPatterns.Singleton;

namespace KB.ConsoleCompanion.Communication
{
    internal class ProtocolClientController : BaseSingleton<ProtocolClientController>
    {
        private IClientProtocolAPI? _client;

        private ProtocolClientController()
        {
            
        }

        public IClientProtocolAPI ClientProtocolAPI
        {
            get 
            { 
                if (_client == null)
                {
                    _client = ProtocolFactory.CreateClient("127.0.0.1", "55555");
                }

                return _client; 
            }
        }
    }
}
