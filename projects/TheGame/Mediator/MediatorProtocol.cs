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
        internal uint UserID;
        internal bool Spawn;
        internal float3 SpawnPosition;
    }

    /// <summary>
    /// Struct for a PlayerUpdate packet.
    /// </summary>
    internal partial struct DataPacketPlayerUpdate
    {
        // Data
        internal uint UserID;
        internal uint Timestamp;
        internal bool PlayerActive;
        internal int PlayerHealth;
        internal float PlayerVelocity;
        internal float3 PlayerPosition;
        internal float3 PlayerRotationX;
        internal float3 PlayerRotationY;
        internal float3 PlayerRotationZ;
    }

    /// <summary>
    /// Struct for a ObjectSpawn packet.
    /// </summary>
    internal partial struct DataPacketObjectSpawn
    {
        // Data
        internal uint UserID;
        internal uint ObjectID;
        internal int ObjectType;
        internal float ObjectVelocity;
        internal float3 ObjectPosition;
        internal float3 ObjectRotationX;
        internal float3 ObjectRotationY;
        internal float3 ObjectRotationZ;
    }

    /// <summary>
    /// Struct for a ObjectUpdate packet.
    /// </summary>
    internal partial struct DataPacketObjectUpdate
    {
        // Data
        internal uint UserID;
        internal uint ObjectID;
        internal int ObjectType;
        internal bool ObjectRemoved;
    }
}
