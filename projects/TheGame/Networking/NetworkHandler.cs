using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkHandler
    {
        // private RenderContext _renderContext;
        private readonly NetworkGUI _networkGUI;

        private NetworkServer _networkServer;
        private NetworkClient _networkClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
        /// </summary>
        /// <param name="rc">The rc.</param>
        public NetworkHandler(RenderContext rc)
        {
            _networkGUI = new NetworkGUI(rc, this);
        }

        /// <summary>
        /// Handles the network.
        /// </summary>
        internal void HandleNetwork()
        {
            _networkGUI.ShowNetworkGUI();

            if (Network.Instance.Config.SysType == SysType.Server)
                _networkServer.HandleMessages();

            if (Network.Instance.Config.SysType == SysType.Client)
                _networkClient.HandleMessages();
        }

        /// <summary>
        /// Creates the server.
        /// </summary>
        /// <returns></returns>
        internal NetworkServer CreateServer()
        {
            _networkServer = new NetworkServer(_networkGUI);
            return _networkServer;
        }

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <returns></returns>
        internal NetworkClient CreateClient()
        {
            _networkClient = new NetworkClient(_networkGUI);
            return _networkClient;
        }
    }
}