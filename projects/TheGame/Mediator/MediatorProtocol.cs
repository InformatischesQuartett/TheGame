using Fusee.Math;

namespace Examples.TheGame
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
    /// Struct for a PlayerSpawn packet.
    /// </summary>
    internal partial struct DataPacketPlayerSpawn
    {
        // Data
        internal int UserID;
        internal bool Spawn;
        internal float3 SpawnPosition;
    }

    /// <summary>
    /// Struct for a PlayerUpdate packet.
    /// </summary>
    internal partial struct DataPacketPlayerUpdate
    {
        // Data
        internal int UserID;
        internal uint Timestamp;
        internal bool PlayerActive;
        internal int PlayerHealth;
        internal float3 PlayerPosition;
        internal float3 PlayerRotation;
        internal float3 PlayerVelocity;
    }

    /// <summary>
    /// Struct for a ObjectSpawn packet.
    /// </summary>
    internal partial struct DataPacketObjectSpawn
    {
        // Data
        internal int UserID;
        internal uint ObjectID;
        internal int ObjectType;
        internal float3 ObjectPosition;
        internal float3 ObjectRotation;
        internal float3 ObjectVelocity;
    }

    /// <summary>
    /// Struct for a ObjectUpdate packet.
    /// </summary>
    internal partial struct DataPacketObjectUpdate
    {
        // Data
        internal int UserID;
        internal uint ObjectID;
        internal int ObjectType;
        internal bool ObjectRemoved;
    }
}
