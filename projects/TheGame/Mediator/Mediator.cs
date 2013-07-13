using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine;

namespace Examples.TheGame
{
    internal class Mediator
    {
        private readonly GameHandler _gameHandler;
        private readonly NetworkHandler _networkHandler;

        private readonly Dictionary<DataPacket, bool> _sendingBuffer;
        private readonly Dictionary<DataPacket, bool> _recevingBuffer;

        private readonly bool _networkActive;

        private int _userID;

        internal int UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                _gameHandler.UserID = value;
            }
        }

        internal int Height { set; get; }
        internal int Width { set; get; }

        /// <summary>
        ///     The last assigned objectID.
        /// </summary>
        private int _objectID;

        /// <summary>
        ///     Amount of objects a client can spawn.
        /// </summary>
        private const int ObjectRange = 16500000;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="rContext">The RenderContext.</param>
        /// <param name="networkActive">
        ///     Network is only used if set to <c>true</c>.
        /// </param>
        internal Mediator(RenderContext rContext, bool networkActive)
        {
            _sendingBuffer = new Dictionary<DataPacket, bool>();
            _recevingBuffer = new Dictionary<DataPacket, bool>();

            _gameHandler = new GameHandler(rContext, this);

            _networkActive = networkActive;
            if (networkActive)
                _networkHandler = new NetworkHandler(rContext, this);

            UserID = (networkActive) ? -1 : 0;
            _objectID = -1;

            _gameHandler.GameState.CurState = GameState.State.InGame;
        }

        /// <summary>
        ///     Updates this instance once a frame.
        /// </summary>
        internal void Update()
        {
            // specific GUI if network is used
            if (_gameHandler.GameState.CurState == GameState.State.StartMenu)
                if (_networkActive)
                {
                    _networkHandler.NetworkGUI();
                    return;
                }

             _gameHandler.Update();
             _gameHandler.Render();
            
            if (_networkActive)
                _networkHandler.HandleNetwork();
        }

        /// <summary>
        ///     Gets a new ObjectID which is based on the UserID
        /// </summary>
        /// <returns></returns>
        internal int GetObjectId()
        {
            if (UserID == -1)
                return -1;

            if (_objectID == -1 || _objectID == ((UserID + 1)*ObjectRange) - 1)
                return _objectID = UserID*ObjectRange;

            return ++_objectID;
        }

        /// <summary>
        ///     Adds data to the sending buffer.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="server">
        ///     Contains server specific data if set to <c>true</c>.
        /// </param>
        internal void AddToSendingBuffer(DataPacket data, bool server)
        {
            _sendingBuffer.Add(data, server);
        }

        /// <summary>
        ///     Gets data from the sending buffer.
        /// </summary>
        /// <returns>
        ///     A KeyValuePair with the data packet and a bool
        ///     if the packet contains server specific data.
        /// </returns>
        internal KeyValuePair<DataPacket, bool> GetFromSendingBuffer()
        {
            var firstOrDefault = _sendingBuffer.FirstOrDefault();
            _sendingBuffer.Remove(_sendingBuffer.Keys.First());

            return firstOrDefault;
        }

        /// <summary>
        ///     Adds data to the receiving buffer.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="server">
        ///     Contains server specific data if set to <c>true</c>.
        /// </param>
        internal void AddToReceivingBuffer(DataPacket data, bool server)
        {
            _recevingBuffer.Add(data, server);
        }

        /// <summary>
        ///     Gets data from the receiving buffer.
        /// </summary>
        /// <returns>
        ///     A KeyValuePair with the data packet and a bool
        ///     if the packet contains server specific data.
        /// </returns>
        internal KeyValuePair<DataPacket, bool> GetFromReceivingBuffer()
        {
            var firstOrDefault = _recevingBuffer.FirstOrDefault();
            _recevingBuffer.Remove(_recevingBuffer.Keys.First());

            return firstOrDefault;
        }

        /// <summary>
        /// Gets the current UNIX timestamp.
        /// </summary>
        /// <returns></returns>
        internal uint GetUnixTimestamp()
        {
            var timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            return (uint)timestamp.TotalSeconds;
        }
    }
}