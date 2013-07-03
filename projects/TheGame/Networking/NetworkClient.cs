using System.Diagnostics;
using Fusee.Engine;

namespace Examples.TheGame.Networking
{
    class NetworkClient
    {
        private readonly Mediator _mediator;
        private readonly NetworkGUI _networkGUI;

        private int _userID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class.
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
        internal void ReceiveKeepAlive(NetworkPacketKeepAlive keepAlive)
        {
            Debug.WriteLine("KeepAlive bekommen. ID: " + keepAlive.KeepAliveID);

            if (_userID != -1)
            {
                var data = new NetworkPacketKeepAlive {KeepAliveID = keepAlive.KeepAliveID, UserID = _userID};
                var packet = NetworkProtocol.MessageEncode(NetworkPacketTypes.KeepAlive, data);

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
                        case NetworkPacketTypes.KeepAlive:
                            ReceiveKeepAlive((NetworkPacketKeepAlive) decodedMessage.Packet);
                            break;

                        case NetworkPacketTypes.PlayerSpawn:
                            var packetPlayerSpawn = (NetworkPacketPlayerSpawn) decodedMessage.Packet;

                            if (!packetPlayerSpawn.Spawn)
                            {
                                Debug.WriteLine("UserID wurde zugewiesen: " + packetPlayerSpawn.UserID);
                                _userID = packetPlayerSpawn.UserID;
                            }
                            else
                            {
                                // TODO: Inform GameHandler about Spawning Position                              
                            }

                            break;

                        case NetworkPacketTypes.PlayerUpdate:
                            // TODO: Inform GameHandler
                            break;

                        case NetworkPacketTypes.ObjectSpawn:
                            // TODO: Inform GameHandler
                            break;

                        case NetworkPacketTypes.ObjectUpdate:
                            // TODO: Inform GameHandler
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
