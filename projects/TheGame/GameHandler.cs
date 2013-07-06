using System.Collections.Generic;
using Examples.TheGame.Entities;
using Fusee.Engine;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    internal class GameHandler
    {
        /// <summary>
        ///Disctionarires mit allen Items und Playern
        /// </summary>
        public static Dictionary<int, GameEntity> Items;

        public static Dictionary<int, Player> Players;

        /// <summary>
        ///State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        internal int UserID { get; set; }

        private readonly Mediator.Mediator _mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        internal GameHandler(RenderContext rc, Mediator.Mediator mediator)
        {
            //pass RenderContext
            _rc = rc;
            _mediator = mediator;

            Items = new Dictionary<int, GameEntity>();
            Players = new Dictionary<int, Player>();
            GameState = new GameState(GameState.State.StartMenu);
        }
    }
}