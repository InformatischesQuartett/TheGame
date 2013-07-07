using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Examples.TheGame.Networking;
using Fusee.Engine;

namespace Examples.TheGame
{
    internal class Mediator
    {
        /// <summary>
        /// The main game handler
        /// </summary>
        private readonly GameHandler _gameHandler;

        /// <summary>
        /// The NetworkHandler
        /// </summary>
        private readonly NetworkHandler _networkHandler;

        /// <summary>
        /// Network is only used if set to <c>true</c>.
        /// </summary>
        private bool _networkActive;

        /// <summary>
        /// Gets or sets the UserID.
        /// </summary>
        /// <value>
        /// The UserID.
        /// </value>
        internal int UserID { get; set; }

        /// <summary>
        /// The last assigned objectID.
        /// </summary>
        private int _objectID;

        /// <summary>
        /// Every client is able to spawn 16,500,000 objects.
        /// </summary>
        private const int Range = 16500000;


        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="rc">The RenderContext.</param>
        /// <param name="networkActive">Network is only used if set to <c>true</c>.</param>
        internal Mediator(RenderContext rc, bool networkActive)
        {
            _gameHandler = new GameHandler(rc, this);

            _networkActive = networkActive;
            if (networkActive)
                _networkHandler = new NetworkHandler(rc, this);

            UserID = 0;
            _objectID = -1;
        }

        /// <summary>
        /// Gets a new ObjectID which is based on the UserID
        /// </summary>
        /// <returns></returns>
        internal int GetObjectId()
        {
            if (_objectID == -1 || _objectID == ((UserID + 1)*Range) - 1)
                return _objectID = UserID*Range;

            return ++_objectID;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
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
    }
}
