using System.Diagnostics;
using Examples.TheGame.Mediator;
using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkClient
    {
        private readonly Mediator.Mediator _mediator;
        private readonly NetworkGUI _networkGUI;

        private int _userID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        /// <param name="networkGUI">The network GUI.</param>
        /// <param name="mediator"></param>
        internal NetworkClient(NetworkGUI networkGUI, Mediator.Mediator mediator)
        {
            _mediator = mediator;
            _networkGUI = networkGUI;

            Network.Instance.Config.SysType = SysType.Client;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.ConnectOnDiscovery = true;
            Network.Instance.Config.DefaultPort = 54954;

            _userID = -1;
        }

        /// <summary>
        /// Startups the client.
        /// </summary>
        internal void Startup()
        {
            Network.Instance.StartPeer();   
        }

        /// <summary>
        /// Connects to the given ip.
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
        /// Handles received KeepAlive packets.
        /// </summary>
        internal void ReceiveKeepAlive(DataPacketKeepAlive keepAlive)
        {
            Debug.WriteLine("KeepAlive bekommen. ID: " + keepAlive.KeepAliveID);

            if (_userID != -1)
            {
                var data = new DataPacketKeepAlive {KeepAliveID = keepAlive.KeepAliveID, UserID = _userID};
                var packet = NetworkProtocol.MessageEncode(DataPacketTypes.KeepAlive, data);

                Network.Instance.SendMessage(packet);
            }
            else
                Debug.WriteLine("Warnung: Keine UserID vom Server bekommen!");
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
                                _mediator.AddToReceivingBuffer(decodedMessage);

                            break;

                        case DataPacketTypes.PlayerUpdate:
                        case DataPacketTypes.ObjectSpawn:
                        case DataPacketTypes.ObjectUpdate:
                            _mediator.AddToReceivingBuffer(decodedMessage);
                            break;
                    }
                }

                if (msg.Type == MessageType.DiscoveryResponse)
                {
                    _networkGUI.ConnectToIp = msg.Sender.Address.ToString();
                }
            }
        }
    }
}
