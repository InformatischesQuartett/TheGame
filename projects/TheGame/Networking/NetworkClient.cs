using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkClient
    {
        private readonly NetworkGUI _networkGUI;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        /// <param name="networkGUI">The network GUI.</param>
        internal NetworkClient(NetworkGUI networkGUI)
        {
            _networkGUI = networkGUI;
        }

        /// <summary>
        /// Handles received keep alive packages.
        /// </summary>
        void ReceiveKeepAlive(NetworkPackageKeepAlive keepAlive)
        {
            Debug.WriteLine("KeepAlive von Spieler " + keepAlive.UserID + ", ID: " +
                            keepAlive.KeepAliveID);

            if (keepAlive.KeepAliveID == _keepAliveID)
                _keepAliveResponses[keepAlive.UserID] = true;
        }

        /// <summary>
        /// Handles incoming messages.
        /// </summary>
        internal void HandleMessages()
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
                    var decodedMessage = NetworkProtocol.MessageDecode(msg);

                    switch (decodedMessage.PackageType)
                    {
                        case NetworkPackageTypes.KeepAlive:
                            ReceiveKeepAlive((NetworkPackageKeepAlive)decodedMessage.Package);
                            break;
                    }
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
    }
}
