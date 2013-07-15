using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Engine;

namespace Examples.TheGame
{
    internal class NetworkClient
    {
        private readonly Mediator _mediator;
        private readonly NetworkGUI _networkGUI;

        private uint _userID;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkClient" /> class.
        /// </summary>
        /// <param name="networkGUI">The network GUI.</param>
        /// <param name="mediator"></param>
        internal NetworkClient(NetworkGUI networkGUI, Mediator mediator)
        {
            _mediator = mediator;
            _networkGUI = networkGUI;

            Network.Instance.Config.SysType = SysType.Client;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.ConnectOnDiscovery = true;
            Network.Instance.Config.DefaultPort = 54954;

            _userID = 0;
        }

        /// <summary>
        ///     Startups the client.
        /// </summary>
        internal void Startup()
        {
            Network.Instance.StartPeer();
        }

        /// <summary>
        ///     Connects to the given ip.
        /// </summary>
        /// <param name="ip">The ip.</param>
        internal void ConnectTo(string ip)
        {
            Network.Instance.StartPeer();

            if (ip.Length > 0 && ip != "Discovery?")
                Network.Instance.OpenConnection(ip, 14242);
            else
                Network.Instance.SendDiscoveryMessage(14242);
        }

        /// <summary>
        ///     Handles received KeepAlive packets.
        /// </summary>
        internal void ReceiveKeepAlive(DataPacketKeepAlive keepAlive)
        {
            if (_userID != 0)
            {
                var data = new DataPacketKeepAlive {KeepAliveID = keepAlive.KeepAliveID, UserID = _userID};
                var packet = NetworkProtocol.MessageEncode(DataPacketTypes.KeepAlive, data);

                Network.Instance.SendMessage(packet, data.MsgDelivery, data.ChannelID);
            }
            else
                Debug.WriteLine("Warnung: Keine UserID vom Server bekommen!");
        }

        /// <summary>
        ///     Handles incoming messages.
        /// </summary>
        internal void HandleMessages()
        {
            // OUTGOING
            KeyValuePair<DataPacket, bool> sendingPacket;
            while ((sendingPacket = _mediator.GetFromSendingBuffer()).Key.Packet != null)
            {
                MessageDelivery msgDelivery;
                int channelID;

                switch (sendingPacket.Key.PacketType)
                {
                    case DataPacketTypes.PlayerUpdate:
                        var playerUpdateData = (DataPacketPlayerUpdate) sendingPacket.Key.Packet;

                        msgDelivery = playerUpdateData.MsgDelivery;
                        channelID = playerUpdateData.ChannelID;

                        var playerUpdatePacket = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerUpdate,
                                                                               playerUpdateData);

                        Network.Instance.SendMessage(playerUpdatePacket, msgDelivery, channelID);

                        break;

                    case DataPacketTypes.ObjectSpawn:
                        var objectSpawnData = (DataPacketObjectSpawn) sendingPacket.Key.Packet;

                        msgDelivery = objectSpawnData.MsgDelivery;
                        channelID = objectSpawnData.ChannelID;

                        var objectSpawnPacket = NetworkProtocol.MessageEncode(DataPacketTypes.ObjectSpawn,
                                                                              objectSpawnData);

                        Network.Instance.SendMessage(objectSpawnPacket, msgDelivery, channelID);

                        break;

                    case DataPacketTypes.ObjectUpdate:
                        var objectUpdateData = (DataPacketObjectUpdate) sendingPacket.Key.Packet;

                        msgDelivery = objectUpdateData.MsgDelivery;
                        channelID = objectUpdateData.ChannelID;

                        var objectUpdatePacket = NetworkProtocol.MessageEncode(DataPacketTypes.PlayerUpdate,
                                                                               objectUpdateData);

                        Network.Instance.SendMessage(objectUpdatePacket, msgDelivery, channelID);

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

                if (msg.Type == MessageType.Data)
                {
                    var decodedMessage = NetworkProtocol.MessageDecode(msg);

                    switch (decodedMessage.PacketType)
                    {
                        case DataPacketTypes.KeepAlive:
                            ReceiveKeepAlive((DataPacketKeepAlive) decodedMessage.Packet);
                            break;

                        case DataPacketTypes.PlayerSpawn:
                            var packetPlayerSpawn = (DataPacketPlayerSpawn) decodedMessage.Packet;

                            if (!packetPlayerSpawn.Spawn)
                            {
                                _userID = packetPlayerSpawn.UserID;
                                _mediator.UserID = _userID;
                            }
                            else
                            {
                                // spawning, so start the game
                                _mediator.AddToReceivingBuffer(decodedMessage, false);

                                if (!_mediator.ActiveGame)
                                    _mediator.StartGame();
                            }

                            break;

                        case DataPacketTypes.PlayerUpdate:
                        case DataPacketTypes.ObjectSpawn:
                        case DataPacketTypes.ObjectUpdate:
                            _mediator.AddToReceivingBuffer(decodedMessage, false);
                            break;
                    }
                }

                if (msg.Type == MessageType.DiscoveryResponse)
                {
                    _networkGUI.ConnectToIp = msg.Sender.RemoteEndPoint.ToString();
                }
            }
        }
    }
}