using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Engine;

namespace Examples.TheGame
{
    internal class Mediator
    {
        private readonly GameHandler _gameHandler;
        private readonly NetworkHandler _networkHandler;

        private readonly List<KeyValuePair<DataPacket, bool>> _sendingBuffer;
        private readonly List<KeyValuePair<DataPacket, bool>> _recevingBuffer;

        private readonly bool _networkActive;

        private uint _userID;

        internal uint UserID
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

        internal bool Blending { set; get; }
        internal bool Fullscreen { set; get; }

        internal bool ActiveGame { get; set; }

        /// <summary>
        ///     The last assigned objectID.
        /// </summary>
        private uint _objectID;

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
            _sendingBuffer = new List<KeyValuePair<DataPacket, bool>>();
            _recevingBuffer = new List<KeyValuePair<DataPacket, bool>>();

            _gameHandler = new GameHandler(rContext, this);
            _gameHandler.AudioInitiated.Play();

            _networkActive = networkActive;

            if (networkActive)
                _networkHandler = new NetworkHandler(rContext, this);

            UserID = 0;
            _objectID = 0;

            if (networkActive)
            {
                ActiveGame = false;
                Blending = true;
                _gameHandler.GameState.CurState = GameState.State.StartMenu;
            } else
                StartGame();
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            ActiveGame = true;

            _gameHandler.AudioConnectionEstablished.Play();

            Blending = true;
            Fullscreen = true;

            _gameHandler.GameState.CurState = GameState.State.InGame;
            _gameHandler.StartGame();
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
                    Blending = true;

                    _networkHandler.NetworkGUI();
                    _networkHandler.HandleNetwork();

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
        internal uint GetObjectId()
        {
            if (_objectID == 0 || _objectID == ((UserID + 1)*ObjectRange) - 1)
                return _objectID = (UserID*ObjectRange)+1;

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
            _sendingBuffer.Add(new KeyValuePair<DataPacket, bool>(data, server));
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
            var emptyPacket = new DataPacket {Packet = null};
            var keyValuePair = new KeyValuePair<DataPacket, bool>(emptyPacket, false);

            if (_sendingBuffer.Count > 0)
            {
                keyValuePair = _sendingBuffer.First();
                _sendingBuffer.Remove(keyValuePair);
            }

            return keyValuePair;
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
            _recevingBuffer.Add(new KeyValuePair<DataPacket, bool>(data, server));
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
            var emptyPacket = new DataPacket { Packet = null };
            var keyValuePair = new KeyValuePair<DataPacket, bool>(emptyPacket, false);

            if (_recevingBuffer.Count > 0)
            {
                keyValuePair = _recevingBuffer.First();
                _recevingBuffer.Remove(keyValuePair);
            }

            return keyValuePair;
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