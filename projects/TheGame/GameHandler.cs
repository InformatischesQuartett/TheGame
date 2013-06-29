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
        public Dictionary<int, GameEntity> Items;
        public static Dictionary<int, Player> Players;

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
        }
    }
}