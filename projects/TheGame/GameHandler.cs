using System;
using System.Collections.Generic;
using Examples.TheGame.Entities;
using Fusee.Engine;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    public class GameHandler
    {
      
        /// <summary>
        ///Disctionarires mit allen Items und Playern
        /// </summary>
        public static Dictionary<int, GameEntity> Items;
        public static Dictionary<int, Player> Players;

        /// <summary>
        ///State Object, contains the current State the Game is in
        /// </summary>
        private GameState _gameState;

        /// <summary>
        ///GameHandler Object, connectioen between Network and GameHandler
        /// </summary>
        private NetworkHandler _networkHandler;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        public GameHandler(RenderContext rc)
        {
            //pass RenderContext
            _rc = rc;
            Items = new Dictionary<int, GameEntity>();
            Players = new Dictionary<int, Player>();
            _gameState = new GameState(GameState.State.StartMenu);
            _networkHandler = new NetworkHandler();
        }
    }
}