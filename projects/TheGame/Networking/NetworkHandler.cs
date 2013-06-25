using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkHandler
    {
        private RenderContext _renderContext;
        private readonly NetworkGUI _networkGUI;

        private readonly NetworkServer _networkServer;
        // private readonly NetworkClient _networkClient;

        public NetworkHandler(RenderContext rc)
        {
            _renderContext = rc;

            _networkGUI = new NetworkGUI(rc);
            _networkServer = new NetworkServer(_networkGUI);
            // _networkClient = new NetworkClient(_networkGUI);
        }

        internal void HandleNetwork()
        {
            _networkGUI.StartupGUI();

            if (Network.Instance.Config.SysType == SysType.Server)
                _networkServer.HandleMessages();

            // if (Network.Instance.Config.SysType == SysType.Client)
            //    _networkServer.HandleMessages();
        }
    }
}