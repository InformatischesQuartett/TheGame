﻿using System.Diagnostics;
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

            Network.Instance.Config.SysType = SysType.Client;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.ConnectOnDiscovery = true;
            Network.Instance.Config.DefaultPort = 54954;
        }

        internal void Startup()
        {
            Network.Instance.StartPeer();   
        }

        internal void ConnectTo(string ip)
        {
            Network.Instance.StartPeer();

            if (ip.Length > 0 && ip != "Discovery?")
                Network.Instance.OpenConnection(ip, 14242);
            else
                Network.Instance.SendDiscoveryMessage(14242);
        }

        /// <summary>
        /// Handles received KeepAlive packets.
        /// </summary>
        internal void ReceiveKeepAlive(NetworkPacketKeepAlive keepAlive)
        {
            Debug.WriteLine("KeepAlive von Spieler " + keepAlive.UserID + ", ID: " +
                            keepAlive.KeepAliveID);

            var msg = NetworkProtocol.MessageEncode(NetworkPacketTypes.KeepAlive, keepAlive.KeepAliveID);
            Network.Instance.SendMessage(msg);
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

                    switch (decodedMessage.PacketType)
                    {
                        case NetworkPacketTypes.KeepAlive:
                            ReceiveKeepAlive((NetworkPacketKeepAlive)decodedMessage.Packet);
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
