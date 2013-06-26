using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Networking
{
    /// <summary>
    /// Struct for a network packet.
    /// </summary>
    internal struct NetworkPacket
    {
        internal NetworkPacketTypes PacketType;
        internal object Packet;
    }

    /// <summary>
    /// Types of packets.
    /// </summary>
    internal enum NetworkPacketTypes
    {
        KeepAlive,
        PlayerSpawn,
        PlayerUpdate,
        ObjectSpawn,
        ObjectUpdate,
    }

    /// <summary>
    /// Struct for a KeepAlive packet.
    /// </summary>
    internal struct NetworkPacketKeepAlive
    {
        internal int UserID;
        internal int KeepAliveID;
    }

    /// <summary>
    /// Struct for a PlayerSpawn packet.
    /// </summary>
    internal struct NetworkPacketPlayerSpawn
    {
        internal int UserID;
        internal float3 SpawnPosition;
    }

    /// <summary>
    /// Struct for a PlayerUpdate packet.
    /// </summary>
    internal struct NetworkPacketPlayerUpdate
    {
        internal int UserID;
        internal float3 PlayerPosition;
        internal float3 PlayerRotation;
        internal float3 PlayerVelocity;
        internal int PlayerHealth;
    }

    /// <summary>
    /// Struct for a ObjectSpawn packet.
    /// </summary>
    internal struct NetworkPacketObjectSpawn
    {
        internal int ObjectID;
        internal int UserID;
        internal float3 ObjectPosition;
        internal float3 ObjectRotation;
        internal float3 ObjectVelocity;
        // internal ... ObjectType
    }

    /// <summary>
    /// Struct for a ObjectUpdate packet.
    /// </summary>
    internal struct NetworkPacketObjectUpdate
    {
        internal int ObjectID;
        internal bool ObjectRemoved;
    }

    /// <summary>
    /// Handles the encoding and decoding of messages.
    /// </summary>
    internal static class NetworkProtocol
    {
        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <returns>An array of bytes to be sent via network.</returns>
        internal static byte[] MessageEncode(NetworkPacketTypes msgType, object packetData)
        {
            // KeepAlive
            if (msgType == NetworkPacketTypes.KeepAlive)
            {
                var data = (NetworkPacketKeepAlive) packetData;
                var keepAliveIDBytes = BitConverter.GetBytes(data.KeepAliveID);
                
                var packet = new byte[keepAliveIDBytes.Length + 2];
                Buffer.BlockCopy(keepAliveIDBytes, 0, packet, 1, keepAliveIDBytes.Length);

                packet[0] = (byte) msgType;                 // PacketType
                packet[1] = (byte) (data.UserID & 255);     // UserID (0 = Server)

                return packet;
            }

            return null;
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="msg"></param>
        internal static NetworkPacket MessageDecode(INetworkMsg msg)
        {
            var decodedMessage = new NetworkPacket();
            
            try
            {
                var msgData = msg.Message.ReadBytes;

                var packetType = (NetworkPacketTypes) msgData[0];
                decodedMessage.PacketType = packetType;

                switch (packetType)
                {
                    case NetworkPacketTypes.KeepAlive:
                        var keepAlivePacket = new NetworkPacketKeepAlive
                            {
                                UserID = msgData[1],
                                KeepAliveID = BitConverter.ToInt32(msgData, 3)
                            };

                        decodedMessage.Packet = keepAlivePacket;

                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR WHILE DECODING MESSAGE: " + e);
            }

            return decodedMessage;
        }
    }
}
