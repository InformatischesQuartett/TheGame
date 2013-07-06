using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Mediator
{
    /// <summary>
    /// Struct for a data packet.
    /// </summary>
    internal struct DataPacket
    {
        internal DataPacketTypes PacketType;
        internal object Packet;
    }

    /// <summary>
    /// Types of packets.
    /// </summary>
    internal enum DataPacketTypes
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
    internal struct DataPacketKeepAlive
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
    internal struct DataPacketPlayerSpawn
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
    internal struct DataPacketPlayerUpdate
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
    internal struct DataPacketObjectSpawn
    {
        // Data
        internal int UserID;
        internal uint ObjectID;
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
    internal struct DataPacketObjectUpdate
    {
        // Data
        internal int UserID;
        internal uint ObjectID;
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
}
