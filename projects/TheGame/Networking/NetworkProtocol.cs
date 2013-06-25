using System;

namespace Examples.TheGame.Networking
{
    /// <summary>
    /// Types of packages.
    /// </summary>
    internal enum NetworkPackageTypes
    {
        KeepAlive,
        PlayerUpdate,
        ObjectSpawn,
        ObjectRemove,
    }

    /// <summary>
    /// Struct for a keep alive package
    /// </summary>
    internal struct NetworkPackageKeepAlive
    {
        internal int UserID;
        internal int KeepAliveID;
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
                Buffer.BlockCopy(keepAliveIDBytes, 0, package, 2, keepAliveIDBytes.Length);

                package[0] = 0;
                package[1] = 1;

                return package;
            }

            return null;
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        internal static void MessageDecode()
        {

        }
    }
}
