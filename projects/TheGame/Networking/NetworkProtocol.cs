using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Networking
{
    /// <summary>
    /// Struct for a network package.
    /// </summary>
    internal struct NetworkPackage
    {
        internal NetworkPackageTypes PackageType;
        internal dynamic Package;
    }

    /// <summary>
    /// Types of packages.
    /// </summary>
    internal enum NetworkPackageTypes
    {
        KeepAlive,
        PlayerSpawn,
        PlayerUpdate,
        ObjectSpawn,
        ObjectUpdate,
    }

    /// <summary>
    /// Struct for a KeepAlive package.
    /// </summary>
    internal struct NetworkPackageKeepAlive
    {
        internal int UserID;
        internal int KeepAliveID;
    }

    /// <summary>
    /// Struct for a PlayerSpawn package.
    /// </summary>
    internal struct NetworkPackagePlayerSpawn
    {
        internal int UserID;
        internal float3 SpawnPosition;
    }

    /// <summary>
    /// Struct for a PlayerUpdate package.
    /// </summary>
    internal struct NetworkPackagePlayerUpdate
    {
        internal int UserID;
        internal float3 PlayerPosition;
        internal float3 PlayerRotation;
        internal float3 PlayerVelocity;
        internal float3 PlayerHealth;
    }

    /// <summary>
    /// Struct for a ObjectSpawn package.
    /// </summary>
    internal struct NetworkPackageObjectSpawn
    {
        internal int ObjectID;
        internal float3 ObjectPosition;
        internal float3 ObjectRotation;
        internal float3 ObjectVelocity;
        // internal ... ObjectType
    }

    /// <summary>
    /// Struct for a ObjectUpdate package.
    /// </summary>
    internal struct NetworkPackageObjectUpdate
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
        internal static byte[] MessageEncode(NetworkPackageTypes msgType, int keepAliveID = 0)
        {
            if (msgType == NetworkPackageTypes.KeepAlive)
            {
                var keepAliveIDBytes = BitConverter.GetBytes(keepAliveID);
                
                var package = new byte[keepAliveIDBytes.Length + 2];
                Buffer.BlockCopy(keepAliveIDBytes, 0, package, 1, keepAliveIDBytes.Length);

                package[0] = (byte) msgType;    // PacketType
                package[1] = 0;                 // UserID = 0 (Server)

                return package;
            }

            return null;
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="msg"></param>
        internal static NetworkPackage MessageDecode(INetworkMsg msg)
        {
            var decodedMessage = new NetworkPackage();
            
            try
            {
                var msgData = msg.Message.ReadBytes;

                var packageType = (NetworkPackageTypes) msgData[0];
                decodedMessage.PackageType = packageType;

                switch (packageType)
                {
                        case NetworkPackageTypes.KeepAlive:

                        var keepAlivePackage = new NetworkPackageKeepAlive
                            {
                                UserID = msgData[1],
                                KeepAliveID = BitConverter.ToInt32(msgData, 3)
                            };

                        decodedMessage.Package = keepAlivePackage;

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
