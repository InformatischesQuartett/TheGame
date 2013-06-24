using System.Diagnostics;
using Fusee.Engine;

namespace Examples.TdM.Networking
{
    class NetworkHandler
    {
        private RenderContext _renderContext;
        private readonly NetworkGUI _networkGUI;

        public NetworkHandler(RenderContext rc)
        {
            _renderContext = rc;
            _networkGUI = new NetworkGUI(rc);
        }

        private void HandleMessages()
        {
            INetworkMsg msg;
            while ((msg = Network.Instance.IncomingMsg) != null)
            {
                if (msg.Type == MessageType.StatusChanged)
                    _networkGUI.RefreshGUITex();

                if (msg.Type == MessageType.DebugMessage)
                {
                    // TODO
                }

                if (msg.Type == MessageType.Data)
                {
                    // TODO
                }

                if (msg.Type == MessageType.DiscoveryRequest)
                {
                    // TODO
                }

                if (msg.Type == MessageType.DiscoveryResponse)
                {
                    _networkGUI.ConnectToIp = msg.Sender.Address.ToString();
                }
            }
        }

        internal void HandleNetwork()
        {
            _networkGUI.StartupGUI();
            HandleMessages();
        }
    }
}