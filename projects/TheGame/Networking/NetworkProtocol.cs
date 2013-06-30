using System;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using ProtoBuf;

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
        // Data
        internal int UserID;
        internal int KeepAliveID;

        // Settings
        internal int ChannelID
        {
            get { return 0; }
        }
        
        internal MessageDelivery MsgDelivery
        {
            get { return MessageDelivery.ReliableSequenced; }
        }
    }

    /// <summary>
    /// Struct for a PlayerSpawn packet.
    /// </summary>
    internal struct NetworkPacketPlayerSpawn
    {
        // Data
        internal int UserID;
        internal bool Spawn;
        internal float3 SpawnPosition;

        // Settings
        internal int ChannelID
        {
            get { return 1; }
        }

        internal MessageDelivery MsgDelivery
        {
            get { return MessageDelivery.ReliableSequenced; }
        }
    }

    /// <summary>
    /// Struct for a PlayerUpdate packet.
    /// </summary>
    internal struct NetworkPacketPlayerUpdate
    {
        // Data
        internal int UserID;
        internal bool PlayerActive;
        internal int PlayerHealth;
        internal float3 PlayerPosition;
        internal float3 PlayerRotation;
        internal float3 PlayerVelocity;

        // Settings
        internal int ChannelID
        {
            get { return 2; }
        }

        internal MessageDelivery MsgDelivery
        {
            get { return MessageDelivery.ReliableSequenced; }
        }
    }

    /// <summary>
    /// Struct for a ObjectSpawn packet.
    /// </summary>
    internal struct NetworkPacketObjectSpawn
    {
        // Data
        internal int UserID;
        internal int ObjectID;
        internal int ObjectType;
        internal float3 ObjectPosition;
        internal float3 ObjectRotation;
        internal float3 ObjectVelocity;

        // Settings
        internal int ChannelID
        {
            get { return 3; }
        }

        internal MessageDelivery MsgDelivery
        {
            get { return MessageDelivery.ReliableOrdered; }
        }
    }

    /// <summary>
    /// Struct for a ObjectUpdate packet.
    /// </summary>
    internal struct NetworkPacketObjectUpdate
    {
        // Data
        internal int ObjectID;
        internal int ObjectType;
        internal bool ObjectRemoved;

        // Settings
        internal int ChannelID
        {
            get { return 3; }
        }

        internal MessageDelivery MsgDelivery
        {
            get { return MessageDelivery.ReliableOrdered; }
        }
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
                Buffer.BlockCopy(keepAliveIDBytes, 0, packet, 2, keepAliveIDBytes.Length);

                packet[0] = (byte) msgType;                 // PacketType
                packet[1] = (byte) (data.UserID & 255);     // UserID (0 = Server)

                return packet;
            }

            // PlayerSpawn
            if (msgType == NetworkPacketTypes.PlayerSpawn)
            {
                var data = (NetworkPacketPlayerSpawn) packetData;           

                byte[] encodedSpawnPosition;
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, data.SpawnPosition);
                    encodedSpawnPosition = stream.ToArray();
                }

                var packet = new byte[encodedSpawnPosition.Length + 3];
                Buffer.BlockCopy(encodedSpawnPosition, 0, packet, 3, encodedSpawnPosition.Length);

                packet[0] = (byte) msgType;                 // PacketType
                packet[1] = (byte) (data.UserID & 255);     // UserID
                packet[2] = (byte) ((data.Spawn) ? 1 : 0);  // Contains SpawnPosition

                return packet;
            }

            // PlayerUpdate
            if (msgType == NetworkPacketTypes.PlayerUpdate)
            {
                var data = (NetworkPacketPlayerUpdate)packetData;

                byte[] encPlayerData;
                using (var stream = new MemoryStream())
                {
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerPosition, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerRotation, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerVelocity, PrefixStyle.Base128);

                    encPlayerData = stream.ToArray();
                }

                var packet = new byte[encPlayerData.Length + 4];
                Buffer.BlockCopy(encPlayerData, 0, packet, 4, encPlayerData.Length);

                packet[0] = (byte) msgType;                         // PacketType
                packet[1] = (byte) (data.UserID & 255);             // UserID
                packet[2] = (byte) ((data.PlayerActive) ? 1 : 0);   // If Player is still active
                packet[3] = (byte) (data.PlayerHealth & 255);       // Health of player
                
                return packet;
            }

            // ObjectSpawn
            if (msgType == NetworkPacketTypes.ObjectSpawn)
            {
                var data = (NetworkPacketObjectSpawn)packetData;

                byte[] encObjectData;
                using (var stream = new MemoryStream())
                {
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectPosition, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectRotation, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectVelocity, PrefixStyle.Base128);

                    encObjectData = stream.ToArray();
                }

                var objectIDBytes = BitConverter.GetBytes(data.ObjectID);

                var packet = new byte[objectIDBytes.Length + encObjectData.Length + 7];
                Buffer.BlockCopy(objectIDBytes, 0, packet, 2, objectIDBytes.Length);
                Buffer.BlockCopy(encObjectData, 0, packet, 7, encObjectData.Length);

                packet[0] = (byte) msgType;                       // PacketType
                packet[1] = (byte) (data.UserID & 255);           // UserID
                packet[6] = (byte) (data.ObjectType & 255);       // Type of Object

                return packet;
            }

            // ObjectUpdate
            if (msgType == NetworkPacketTypes.ObjectUpdate)
            {
                var data = (NetworkPacketObjectUpdate)packetData;
                var objectIDBytes = BitConverter.GetBytes(data.ObjectID);

                var packet = new byte[objectIDBytes.Length + 3];
                Buffer.BlockCopy(objectIDBytes, 0, packet, 1, objectIDBytes.Length);

                packet[0] = (byte) msgType;                         // PacketType
                packet[5] = (byte) (data.ObjectType & 255);         // Type of Object
                packet[6] = (byte)((data.ObjectRemoved) ? 1 : 0);   // If object has been removed

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

                // KeepAlive
                if (packetType == NetworkPacketTypes.KeepAlive)
                {
                    var keepAlivePacket = new NetworkPacketKeepAlive
                                              {
                                                  UserID = msgData[1],
                                                  KeepAliveID = BitConverter.ToInt32(msgData, 2)
                                              };

                    decodedMessage.Packet = keepAlivePacket;
                }

                // PlayerSpawn
                if (packetType == NetworkPacketTypes.PlayerSpawn)
                {
                    float3 decodedSpawnPosition;
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 3, msgData.Length - 3);
                        ms.Position = 0;
                        decodedSpawnPosition = Serializer.Deserialize<float3>(ms);
                    }

                    var playerSpawnPacket = new NetworkPacketPlayerSpawn
                                                {
                                                    UserID = msgData[1],
                                                    Spawn = (msgData[2] == 1),
                                                    SpawnPosition = decodedSpawnPosition
                                                };

                    decodedMessage.Packet = playerSpawnPacket;
                }

                // PlayerUpdate
                if (packetType == NetworkPacketTypes.PlayerUpdate)
                {
                    float3 decPlayerPosition;
                    float3 decPlayerRotation;
                    float3 decPlayerVelocity;

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 4, msgData.Length - 4);
                        ms.Position = 0;

                        decPlayerPosition = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decPlayerRotation = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decPlayerVelocity = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                    }

                    var playerUpdatePacket = new NetworkPacketPlayerUpdate
                                                 {
                                                     UserID = msgData[1],
                                                     PlayerActive = (msgData[2] == 1),
                                                     PlayerHealth = msgData[3],
                                                     PlayerPosition = decPlayerPosition,
                                                     PlayerRotation = decPlayerRotation,
                                                     PlayerVelocity = decPlayerVelocity
                                                 };

                    decodedMessage.Packet = playerUpdatePacket;
                }

                // ObjectSpawn
                if (packetType == NetworkPacketTypes.ObjectSpawn)
                {
                    float3 decObjectPosition;
                    float3 decObjectRotation;
                    float3 decObjectVelocity;

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 7, msgData.Length - 7);
                        ms.Position = 0;

                        decObjectPosition = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decObjectRotation = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decObjectVelocity = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                    }

                    var objectSpawnPacket = new NetworkPacketObjectSpawn
                                                {
                                                    UserID = msgData[1],
                                                    ObjectID = BitConverter.ToInt32(msgData, 2),
                                                    ObjectType = msgData[6],
                                                    ObjectPosition = decObjectPosition,
                                                    ObjectRotation = decObjectRotation,
                                                    ObjectVelocity = decObjectVelocity
                                                };

                    decodedMessage.Packet = objectSpawnPacket;
                }

                // ObjectUpdate
                if (packetType == NetworkPacketTypes.ObjectUpdate)
                {
                    var objectUpdatePacket = new NetworkPacketObjectUpdate
                                              {
                                                  ObjectID = BitConverter.ToInt32(msgData, 1),
                                                  ObjectType = msgData[5],
                                                  ObjectRemoved = (msgData[6] == 1)
                                              };

                    decodedMessage.Packet = objectUpdatePacket;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error while decoding packet: " + e);
            }

            return decodedMessage;
        }
    }
}