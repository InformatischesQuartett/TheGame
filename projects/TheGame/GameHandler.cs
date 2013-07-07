using System.Collections.Generic;
using Examples.TheGame.Entities;
using Examples.TheGame.Networking;
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
        public static Dictionary<int, HealthItem> HealthItems;
        public static Dictionary<int, Bullet> Bullets;
        public static Dictionary<int, Player> Players;

        /// <summary>
        ///State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        private readonly Mediator _mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        internal GameHandler(RenderContext rc, Mediator mediator)
        {
            //pass RenderContext
            _rc = rc;
            _mediator = mediator;

            HealthItems = new Dictionary<int, HealthItem>();
            Bullets = new Dictionary<int, Bullet>();
            Players = new Dictionary<int, Player>();

            GameState = new GameState(GameState.State.StartMenu);
        }

        internal void Update()
        {
            foreach (var go in HealthItems)
                go.Value.Update();
            foreach (var go in Bullets)
                go.Value.Update();
            foreach (var go in Players)
                go.Value.Update();
        }

        internal void Render()
        {
            foreach (var go in HealthItems)
                go.Value.RenderUpdate(_rc);
            foreach (var go in Bullets)
                go.Value.RenderUpdate(_rc);
            foreach (var go in Players)
                go.Value.RenderUpdate(_rc);
        }
    }
}