using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Networking
{
    class NetworkServer
    {
        private readonly NetworkGUI _networkGUI;

        private readonly Dictionary<int, INetworkConnection> _userIDs; 

        private readonly Timer _keepAliveTimer;
        private readonly Dictionary<INetworkConnection, bool> _keepAliveResponses;
        private const int KeepAliveInterval = 5000;
        private int _keepAliveID;

        private readonly Random _random;

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

            _userIDs = new Dictionary<int, INetworkConnection>();
            Network.Instance.OnConnectionUpdate += ConnectionUpdate;

            _random = new Random();
            _keepAliveResponses = new Dictionary<INetworkConnection, bool>();

            _keepAliveTimer = new Timer(KeepAliveInterval);
            _keepAliveTimer.Elapsed += SendKeepAlive;
            _keepAliveTimer.Enabled = true;
        }

        /// <summary>
        /// Startups the server.
        /// </summary>
        internal void Startup()
        {
            Network.Instance.StartPeer();
        }

        /// <summary>
        /// Sends a keep alive packet to every client and disconnects inactive clients.
        /// </summary>
        internal void SendKeepAlive(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (Network.Instance.Connections.Count == 0)
                return;

            foreach (var keepAliveResponse in _keepAliveResponses)
                if (!keepAliveResponse.Value)
                    keepAliveResponse.Key.Disconnect();

            // new KeepAlive messages to all clients
            _keepAliveID = _random.Next(10000000, 100000000);

            var data = new NetworkPacketKeepAlive {KeepAliveID = _keepAliveID, UserID = 0};
            var packet = NetworkProtocol.MessageEncode(NetworkPacketTypes.KeepAlive, data);
            Network.Instance.SendMessage(packet, data.MsgDelivery, data.ChannelID);

            _keepAliveResponses.Clear();
            foreach (var userID in _userIDs)
                _keepAliveResponses.Add(userID.Value, false);
        }

        /// <summary>
        /// Handles received keep alive packets.
        /// </summary>
        void ReceiveKeepAlive(NetworkPacketKeepAlive keepAlive)
        {
            if (keepAlive.KeepAliveID == _keepAliveID)
                if (_userIDs.ContainsKey(keepAlive.UserID))
                    _keepAliveResponses[_userIDs[keepAlive.UserID]] = true;
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
            }
        }

        /// <summary>
        /// EventHandler for updates on connections.
        /// </summary>
        /// <param name="connectionStatus">The connection status.</param>
        /// <param name="senderConnection">The corresponding connection.</param>
        void ConnectionUpdate(ConnectionStatus connectionStatus, INetworkConnection senderConnection)
        {
            if (connectionStatus == ConnectionStatus.Connected)
            {
                var newUserID = _random.Next(1, 256);
                while (_userIDs.ContainsKey(newUserID))
                    newUserID = _random.Next(1, 256);

                _userIDs.Add(newUserID, senderConnection);

                // Inform client about his UserID
                var data = new NetworkPacketPlayerSpawn
                    {
                        UserID = newUserID,
                        Spawn = false,
                        SpawnPosition = new float3(0, 0, 0)
                    };

                var packet = NetworkProtocol.MessageEncode(NetworkPacketTypes.PlayerSpawn, data);
                senderConnection.SendMessage(packet, data.MsgDelivery, data.ChannelID);
            }

            if (connectionStatus == ConnectionStatus.Disconnected)
            {
                if (_userIDs.ContainsValue(senderConnection))
                {
                    var item = _userIDs.First(kvp => kvp.Value == senderConnection);
                    _userIDs.Remove(item.Key);
                }

                // TODO: Inform other players.
                // --
            }
        }
    }
}
