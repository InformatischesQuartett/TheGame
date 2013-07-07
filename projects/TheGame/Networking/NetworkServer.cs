using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class NetworkServer
    {
        private readonly Mediator _mediator;
        private readonly NetworkGUI _networkGUI;

        private readonly Dictionary<int, INetworkConnection> _userIDs;

        private readonly Timer _keepAliveTimer;
        private readonly Dictionary<INetworkConnection, bool> _keepAliveResponses;
        private const int KeepAliveInterval = 5000;
        private int _keepAliveID;

        private readonly Random _random;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkServer" /> class.
        /// </summary>
        /// <param name="networkGUI">The network GUI.</param>
        /// <param name="mediator"></param>
        internal NetworkServer(NetworkGUI networkGUI, Mediator mediator)
        {
            _mediator = mediator;
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

            _mediator.UserID = 0;
        }

        /// <summary>
        ///     Startups the server.
        /// </summary>
        internal void Startup()
        {
            Network.Instance.StartPeer();
        }

        /// <summary>
        ///     Sends a keep alive packet to every client and disconnects inactive clients.
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

            var data = new DataPacketKeepAlive {KeepAliveID = _keepAliveID, UserID = 0};
            var packet = NetworkProtocol.MessageEncode(DataPacketTypes.KeepAlive, data);
            Network.Instance.SendMessage(packet, data.MsgDelivery, data.ChannelID);

            _keepAliveResponses.Clear();
            foreach (var userID in _userIDs)
                _keepAliveResponses.Add(userID.Value, false);
        }

        /// <summary>
        ///     Handles received keep alive packets.
        /// </summary>
        private void ReceiveKeepAlive(DataPacketKeepAlive keepAlive)
        {
            if (keepAlive.KeepAliveID == _keepAliveID)
                if (_userIDs.ContainsKey(keepAlive.UserID))
                    _keepAliveResponses[_userIDs[keepAlive.UserID]] = true;
        }

        /// <summary>
        ///     Handles incoming messages.
        /// </summary>
        internal void HandleMessages()
        {
            MessageDelivery msgDelivery;
            int channelID;

            // OUTGOING
            KeyValuePair<DataPacket, bool> sendingPacket;
            while ((sendingPacket = _mediator.GetFromSendingBuffer()).Key.Packet != null)
            {
                int userID;

                switch (sendingPacket.Key.PacketType)
                {
                    case DataPacketTypes.PlayerSpawn:
                        var playerSpawnData = (DataPacketPlayerSpawn) sendingPacket.Key.Packet;

                        if (sendingPacket.Value)
                        {
                            userID = playerSpawnData.UserID;
                            msgDelivery = playerSpawnData.MsgDelivery;
                            channelID = playerSpawnData.ChannelID;

                            var playerSpawnPacket = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerSpawn,
                                                                                  playerSpawnData);

                            _userIDs[userID].SendMessage(playerSpawnPacket, msgDelivery, channelID);
                        }

                        break;

                    case DataPacketTypes.PlayerUpdate:
                        var playerUpdateData = (DataPacketPlayerUpdate) sendingPacket.Key.Packet;

                        userID = playerUpdateData.UserID;
                        msgDelivery = playerUpdateData.MsgDelivery;
                        channelID = playerUpdateData.ChannelID;

                        var playerUpdatePacket = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerUpdate,
                                                                               playerUpdateData);

                        foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                            connection.Value.SendMessage(playerUpdatePacket, msgDelivery, channelID);

                        break;

                    case DataPacketTypes.ObjectSpawn:
                        var objectSpawnData = (DataPacketObjectSpawn) sendingPacket.Key.Packet;

                        userID = objectSpawnData.UserID;
                        msgDelivery = objectSpawnData.MsgDelivery;
                        channelID = objectSpawnData.ChannelID;

                        var objectSpawnPacket = NetworkProtocol.MessageEncode(DataPacketTypes.ObjectSpawn,
                                                                              objectSpawnData);

                        foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                            connection.Value.SendMessage(objectSpawnPacket, msgDelivery, channelID);

                        break;

                    case DataPacketTypes.ObjectUpdate:
                        var objectUpdateData = (DataPacketObjectUpdate) sendingPacket.Key.Packet;

                        userID = objectUpdateData.UserID;
                        msgDelivery = objectUpdateData.MsgDelivery;
                        channelID = objectUpdateData.ChannelID;

                        var objectUpdatePacket = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerUpdate,
                                                                               objectUpdateData);

                        foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                            connection.Value.SendMessage(objectUpdatePacket, msgDelivery, channelID);

                        break;
                }
            }

            // INCOMING
            INetworkMsg msg;

            while ((msg = Network.Instance.IncomingMsg) != null)
            {
                if (msg.Type == MessageType.StatusChanged)
                    _networkGUI.RefreshGUITex();

                if (msg.Type == MessageType.DebugMessage)
                {
                    // TODO
                }

                if (msg.Type == MessageType.DiscoveryRequest)
                {
                    // TODO
                }

                if (msg.Type == MessageType.Data)
                {
                    int userID;
                    var decodedMessage = NetworkProtocol.MessageDecode(msg);

                    switch (decodedMessage.PacketType)
                    {
                        case DataPacketTypes.KeepAlive:
                            ReceiveKeepAlive((DataPacketKeepAlive) decodedMessage.Packet);
                            break;

                        case DataPacketTypes.PlayerUpdate:
                            userID = ((DataPacketPlayerUpdate) decodedMessage.Packet).UserID;

                            // inform GameHandler
                            _mediator.AddToReceivingBuffer(decodedMessage, false);

                            // forward packet to all other clients
                            msgDelivery = ((DataPacketPlayerUpdate) decodedMessage.Packet).MsgDelivery;
                            channelID = ((DataPacketPlayerUpdate) decodedMessage.Packet).ChannelID;

                            foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                                connection.Value.SendMessage(msg.Message.ReadBytes, msgDelivery, channelID);

                            break;

                        case DataPacketTypes.ObjectSpawn:
                            userID = ((DataPacketObjectSpawn) decodedMessage.Packet).UserID;

                            // inform GameHandler
                            _mediator.AddToReceivingBuffer(decodedMessage, false);

                            // forward packet to all other clients
                            msgDelivery = ((DataPacketObjectSpawn) decodedMessage.Packet).MsgDelivery;
                            channelID = ((DataPacketObjectSpawn) decodedMessage.Packet).ChannelID;

                            foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                                connection.Value.SendMessage(msg.Message.ReadBytes, msgDelivery, channelID);
                            break;

                        case DataPacketTypes.ObjectUpdate:
                            userID = ((DataPacketObjectUpdate) decodedMessage.Packet).UserID;

                            // inform GameHandler
                            _mediator.AddToReceivingBuffer(decodedMessage, false);

                            // forward packet to all other clients
                            msgDelivery = ((DataPacketObjectUpdate) decodedMessage.Packet).MsgDelivery;
                            channelID = ((DataPacketObjectUpdate) decodedMessage.Packet).ChannelID;

                            foreach (var connection in _userIDs.Where(connection => connection.Key != userID))
                                connection.Value.SendMessage(msg.Message.ReadBytes, msgDelivery, channelID);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     EventHandler for updates on connections.
        /// </summary>
        /// <param name="connectionStatus">The connection status.</param>
        /// <param name="senderConnection">The corresponding connection.</param>
        private void ConnectionUpdate(ConnectionStatus connectionStatus, INetworkConnection senderConnection)
        {
            if (connectionStatus == ConnectionStatus.Connected)
            {
                var newUserID = _random.Next(1, 256);
                while (_userIDs.ContainsKey(newUserID))
                    newUserID = _random.Next(1, 256);

                _userIDs.Add(newUserID, senderConnection);

                // inform client about his UserID
                var data = new DataPacketPlayerSpawn
                    {
                        UserID = newUserID,
                        Spawn = false,
                        SpawnPosition = new float3(0, 0, 0)
                    };

                var packet = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerSpawn, data);
                senderConnection.SendMessage(packet, data.MsgDelivery, data.ChannelID);

                // inform GameHandler and ask for SpawnPosition
                var gameHandlerPacket = new DataPacket {PacketType = DataPacketTypes.PlayerSpawn, Packet = data};
                _mediator.AddToReceivingBuffer(gameHandlerPacket, true);
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