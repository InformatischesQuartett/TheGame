using System;
using System.Collections.Generic;
using System.Timers;
using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkServer
    {
        private readonly NetworkGUI _networkGUI;

        private int _connectionCount;

        private Timer _keepAliveTimer;
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
            _networkGUI = _networkGUI;

            _keepAliveRand = new Random();
            _keepAliveTimer = new Timer(60000);
            _keepAliveResponses = new List<bool>();
        }

        /// <summary>
        /// Sends a keep alive package to every client and disconnects inactive clients.
        /// </summary>
        void SendKeepAlive()
        {
            for (var i = 0; i < _keepAliveResponses.Count; i++)
            {
                if (!_keepAliveResponses[i])
                {
                    // DISCONNECT PLAYER #i
                }   
            }

            // new KeepAlive messages to everyone
            _keepAliveID = _keepAliveRand.Next(10000000, 100000000);
            NetworkProtocol.MessageEncode(NetworkPackageTypes.KeepAlive, _keepAliveID);

            // Network.Instance.SendToAll();

            _keepAliveResponses.Clear();
            for (var i = 0; i < _connectionCount; i++)
            {
                _keepAliveResponses.Add(false);
            }
        }

        /// <summary>
        /// Handles received keep alive packages.
        /// </summary>
        void ReceiveKeepAlive(NetworkPackageKeepAlive keepAlive)
        {
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
                            ReceiveKeepAlive((NetworkPackageKeepAlive) decodedMessage.Package);
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
