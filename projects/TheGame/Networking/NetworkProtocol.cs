﻿using System;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using ProtoBuf;

namespace Examples.TheGame
{
    /// <summary>
    ///     Struct for a KeepAlive packet.
    /// </summary>
    internal struct DataPacketKeepAlive
    {
        // Data
        internal uint UserID;
        internal int KeepAliveID;
        internal uint Timestamp;

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
    ///     Struct for a PlayerSpawn packet.
    /// </summary>
    internal partial struct DataPacketPlayerSpawn
    {
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
    ///     Struct for a PlayerUpdate packet.
    /// </summary>
    internal partial struct DataPacketPlayerUpdate
    {
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
    ///     Struct for a ObjectSpawn packet.
    /// </summary>
    internal partial struct DataPacketObjectSpawn
    {
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
    ///     Struct for a ObjectUpdate packet.
    /// </summary>
    internal partial struct DataPacketObjectUpdate
    {
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
    ///     Handles the encoding and decoding of messages.
    /// </summary>
    internal static class NetworkProtocol
    {
        /// <summary>
        ///     Encodes the message.
        /// </summary>
        /// <returns>An array of bytes to be sent via network.</returns>
        internal static byte[] MessageEncode(DataPacketTypes msgType, object packetData)
        {
            // KeepAlive
            if (msgType == DataPacketTypes.KeepAlive)
            {
                var data = (DataPacketKeepAlive) packetData;

                var keepAliveIDBytes = BitConverter.GetBytes(data.KeepAliveID);
                var timestampBytes = BitConverter.GetBytes(data.Timestamp);

                var packet = new byte[keepAliveIDBytes.Length + timestampBytes.Length + 2];
                Buffer.BlockCopy(keepAliveIDBytes, 0, packet, 2, keepAliveIDBytes.Length);
                Buffer.BlockCopy(timestampBytes, 0, packet, 6, timestampBytes.Length);

                packet[0] = (byte) msgType;             // PacketType
                packet[1] = (byte) (data.UserID & 255); // UserID (0 = Server)

                return packet;
            }

            // PlayerSpawn
            if (msgType == DataPacketTypes.PlayerSpawn)
            {
                var data = (DataPacketPlayerSpawn) packetData;

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
            if (msgType == DataPacketTypes.PlayerUpdate)
            {
                var data = (DataPacketPlayerUpdate) packetData;

                var timestampBytes = BitConverter.GetBytes(data.Timestamp);
                var velocityBytes = BitConverter.GetBytes(data.PlayerVelocity);

                byte[] encPlayerData;
                using (var stream = new MemoryStream())
                {
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerPosition, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerRotationX, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerRotationY, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.PlayerRotationZ, PrefixStyle.Base128);

                    encPlayerData = stream.ToArray();
                }

                var packet = new byte[timestampBytes.Length + velocityBytes.Length + encPlayerData.Length + 4];

                Buffer.BlockCopy(timestampBytes, 0, packet, 2, timestampBytes.Length);
                Buffer.BlockCopy(velocityBytes, 0, packet, 8, velocityBytes.Length);
                Buffer.BlockCopy(encPlayerData, 0, packet, 12, encPlayerData.Length);

                packet[0] = (byte) msgType;                         // PacketType
                packet[1] = (byte) (data.UserID & 255);             // UserID
                packet[6] = (byte) ((data.PlayerActive) ? 1 : 0);   // If Player is still active
                packet[7] = (byte) (data.PlayerHealth & 255);       // Health of player

                return packet;
            }

            // ObjectSpawn
            if (msgType == DataPacketTypes.ObjectSpawn)
            {
                var data = (DataPacketObjectSpawn) packetData;

                byte[] encObjectData;
                using (var stream = new MemoryStream())
                {
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectPosition, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectRotationX, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectRotationY, PrefixStyle.Base128);
                    Serializer.SerializeWithLengthPrefix(stream, data.ObjectRotationZ, PrefixStyle.Base128);

                    encObjectData = stream.ToArray();
                }

                var objectIDBytes = BitConverter.GetBytes(data.ObjectID);
                var velocityBytes = BitConverter.GetBytes(data.ObjectVelocity);

                var packet = new byte[objectIDBytes.Length + velocityBytes.Length + encObjectData.Length + 3];
                Buffer.BlockCopy(objectIDBytes, 0, packet, 2, objectIDBytes.Length);
                Buffer.BlockCopy(velocityBytes, 0, packet, 7, velocityBytes.Length);
                Buffer.BlockCopy(encObjectData, 0, packet, 11, encObjectData.Length);

                packet[0] = (byte) msgType;                 // PacketType
                packet[1] = (byte) (data.UserID & 255);     // UserID
                packet[6] = (byte) (data.ObjectType & 255); // Type of Object

                return packet;
            }

            // ObjectUpdate
            if (msgType == DataPacketTypes.ObjectUpdate)
            {
                var data = (DataPacketObjectUpdate) packetData;
                var objectIDBytes = BitConverter.GetBytes(data.ObjectID);

                var packet = new byte[objectIDBytes.Length + 4];
                Buffer.BlockCopy(objectIDBytes, 0, packet, 2, objectIDBytes.Length);

                packet[0] = (byte) msgType;                         // PacketType
                packet[2] = (byte) (data.UserID & 255);             // UserID
                packet[6] = (byte) (data.ObjectType & 255);         // Type of Object
                packet[7] = (byte) ((data.ObjectRemoved) ? 1 : 0);  // If object has been removed

                return packet;
            }

            return null;
        }

        /// <summary>
        ///     Decodes the message.
        /// </summary>
        /// <param name="msg"></param>
        internal static DataPacket MessageDecode(INetworkMsg msg)
        {
            var decodedMessage = new DataPacket();

            try
            {
                var msgData = msg.Message.ReadBytes;

                var packetType = (DataPacketTypes) msgData[0];
                decodedMessage.PacketType = packetType;

                // KeepAlive
                if (packetType == DataPacketTypes.KeepAlive)
                {
                    var keepAlivePacket = new DataPacketKeepAlive
                        {
                            UserID = msgData[1],
                            KeepAliveID = BitConverter.ToInt32(msgData, 2),
                            Timestamp = BitConverter.ToUInt32(msgData, 6)
                        };

                    decodedMessage.Packet = keepAlivePacket;
                }

                // PlayerSpawn
                if (packetType == DataPacketTypes.PlayerSpawn)
                {
                    float3 decodedSpawnPosition;
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 3, msgData.Length - 3);
                        ms.Position = 0;
                        decodedSpawnPosition = Serializer.Deserialize<float3>(ms);
                    }

                    var playerSpawnPacket = new DataPacketPlayerSpawn
                        {
                            UserID = msgData[1],
                            Spawn = (msgData[2] == 1),
                            SpawnPosition = decodedSpawnPosition
                        };

                    decodedMessage.Packet = playerSpawnPacket;
                }

                // PlayerUpdate
                if (packetType == DataPacketTypes.PlayerUpdate)
                {
                    float3 decPlayerPosition;
                    float3 decPlayerRotationX;
                    float3 decPlayerRotationY;
                    float3 decPlayerRotationZ;

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 12, msgData.Length - 12);
                        ms.Position = 0;

                        decPlayerPosition = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decPlayerRotationX = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decPlayerRotationY = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decPlayerRotationZ = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                    }

                    var playerUpdatePacket = new DataPacketPlayerUpdate
                        {
                            UserID = msgData[1],
                            Timestamp = BitConverter.ToUInt32(msgData, 2),
                            PlayerActive = (msgData[6] == 1),
                            PlayerHealth = msgData[7],
                            PlayerVelocity = BitConverter.ToSingle(msgData, 8),
                            PlayerPosition = decPlayerPosition,
                            PlayerRotationX = decPlayerRotationX,
                            PlayerRotationY = decPlayerRotationY,
                            PlayerRotationZ = decPlayerRotationZ,
                        };

                    decodedMessage.Packet = playerUpdatePacket;
                }

                // ObjectSpawn
                if (packetType == DataPacketTypes.ObjectSpawn)
                {
                    float3 decObjectPosition;
                    float3 decObjectRotationX;
                    float3 decObjectRotationY;
                    float3 decObjectRotationZ;

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msgData, 11, msgData.Length - 11);
                        ms.Position = 0;

                        decObjectPosition = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decObjectRotationX = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decObjectRotationY = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                        decObjectRotationZ = Serializer.DeserializeWithLengthPrefix<float3>(ms, PrefixStyle.Base128);
                    }

                    var objectSpawnPacket = new DataPacketObjectSpawn
                        {
                            UserID = msgData[1],
                            ObjectID = BitConverter.ToUInt32(msgData, 2),
                            ObjectType = msgData[6],
                            ObjectVelocity = BitConverter.ToSingle(msgData, 7),
                            ObjectPosition = decObjectPosition,
                            ObjectRotationX = decObjectRotationX,
                            ObjectRotationY = decObjectRotationY,
                            ObjectRotationZ = decObjectRotationZ
                        };

                    decodedMessage.Packet = objectSpawnPacket;
                }

                // ObjectUpdate
                if (packetType == DataPacketTypes.ObjectUpdate)
                {
                    var objectUpdatePacket = new DataPacketObjectUpdate
                        {
                            UserID = msgData[1],
                            ObjectID = BitConverter.ToUInt32(msgData, 2),
                            ObjectType = msgData[6],
                            ObjectRemoved = (msgData[7] == 1)
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