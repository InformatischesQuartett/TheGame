using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkServer
    {
        private readonly NetworkGUI _networkGUI;

        private int _connectionCount;

        private Timer _keepAliveTimer;
        private long _keepAliveTimestamp;
        private List<bool> _keepAliveResponses;
        private const int KeepAliveTimeout = 60000;

        private Random _keepAliveRand;
        private int _keepAliveID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServer"/> class.
        /// </summary>
        /// <param name="networkGUI">The network GUI.</param>
        internal NetworkServer(NetworkGUI networkGUI)
        {
            _networkGUI = networkGUI;

            Network.Instance.Config.SysType = SysType.Server;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.DefaultPort = 14242;

            _keepAliveRand = new Random();
            _keepAliveTimer = new Timer(KeepAliveTimeout);
            _keepAliveResponses = new List<bool>();
        }

        internal void Startup()
        {
            Network.Instance.StartPeer();
        }

        /// <summary>
        /// Sends a keep alive packet to every client and disconnects inactive clients.
        /// </summary>
        internal void SendKeepAlive()
        {
            for (var i = 0; i < _keepAliveResponses.Count; i++)
            {
                if (!_keepAliveResponses[i])
                {
                    // DISCONNECT PLAYER #i
                    Debug.WriteLine("Kein KeepAlive von Spieler " + i + " bekommen.");
                }   
            }

            // new KeepAlive messages to all clients
            _keepAliveID = _keepAliveRand.Next(10000000, 100000000);

            var data = new NetworkPacketKeepAlive {KeepAliveID = _keepAliveID, UserID = 0};
            NetworkProtocol.MessageEncode(NetworkPacketTypes.KeepAlive, data);

            // Network.Instance.SendToAll();
            _keepAliveTimestamp = DateTime.Now.Ticks;

            _keepAliveResponses.Clear();
            for (var i = 0; i < _connectionCount; i++)
            {
                _keepAliveResponses.Add(false);
            }
        }

        /// <summary>
        /// Handles received keep alive packets.
        /// </summary>
        void ReceiveKeepAlive(NetworkPacketKeepAlive keepAlive)
        {
            var timeDiff = DateTime.Now.Ticks - _keepAliveTimestamp;
            Debug.WriteLine("KeepAlive von Spieler " + keepAlive.UserID + ", Zeit: " + timeDiff + ", ID: " +
                            keepAlive.KeepAliveID + " (soll: " +
                            _keepAliveID + ")");

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

                    switch (decodedMessage.PacketType)
                    {
                        case NetworkPacketTypes.KeepAlive:
                            ReceiveKeepAlive((NetworkPacketKeepAlive) decodedMessage.Packet);
                            break;
                    }
                }

                if (msg.Type == MessageType.DiscoveryRequest)
                {
                    // TODO
                }

                if (msg.Type == MessageType.DiscoveryResponse)
                {
                    // TODO
                }
            }
        }
    }
}
