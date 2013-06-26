using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkHandler
    {
        // private RenderContext _renderContext;
        private readonly NetworkGUI _networkGUI;

        private NetworkServer _networkServer;
        private NetworkClient _networkClient;

        public NetworkHandler(RenderContext rc)
        {
            _networkGUI = new NetworkGUI(rc, this);
        }

        internal void HandleNetwork()
        {
            _networkGUI.StartupGUI();

            if (Network.Instance.Config.SysType == SysType.Server)
                _networkServer.HandleMessages();

            if (Network.Instance.Config.SysType == SysType.Client)
                _networkClient.HandleMessages();
        }

        internal NetworkServer CreateServer()
        {
            _networkServer = new NetworkServer(_networkGUI);
            return _networkServer;
        }

        internal NetworkClient CreateClient()
        {
            _networkClient = new NetworkClient(_networkGUI);
            return _networkClient;
        }
    }
}