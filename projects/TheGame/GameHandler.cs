using System;
using System.Collections.Generic;
using Examples.TheGame.GameStates;
using Fusee.Engine;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    public class GameHandler
    {
        /// <summary>
        ///     A dictionary storing all game states that are known to the game
        /// </summary>
        private readonly Dictionary<GameState, AbstractGameState> _gameStates;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        /// <summary>
        ///     Currently active and updated game state
        /// </summary>
        private AbstractGameState _curGameState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameHandler" /> class.
        /// </summary>
        /// <param name="rc">Render context of the game window</param>
        public GameHandler(RenderContext rc)
        {
            //pass RenderContext
            _rc = rc;

            // TODO: NetworkManager initalize

            // Initialize list of all game states
            _gameStates = new Dictionary<GameState, AbstractGameState>();
            InitStatesList(_gameStates);
        }

        /// <summary>
        ///     Inits the list of all states the game knows
        /// </summary>
        /// <param name="list">The list to add the states to</param>
        private void InitStatesList(Dictionary<GameState, AbstractGameState> list)
        {
            // TODO: Add all other game states that are being implemented
            list.Add(GameState.InGame, new InGameState());

            // Set the initial game state to use
            _curGameState = _gameStates[GameState.InGame];
        }

        /// <summary>
        ///     Update entity states, is called once per frame
        /// </summary>
        public void UpdateStates()
        {
            if (_curGameState != null)
                _curGameState.Update();
        }

        /// <summary>
        ///     Is called once per frame to updeate rendering
        /// </summary>
        public void RenderAFrame()
        {
            if (_curGameState != null)
                _curGameState.Render(_rc);
        }

        /// <summary>
        ///     Switches the state of the game.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        /// <exception cref="ArgumentException">The passed game state is not known.</exception>
        public void SwitchGameState(GameState gameState)
        {
            if (_gameStates.ContainsKey(gameState))
            {
                _curGameState = _gameStates[gameState];
            }
            else
            {
                throw new ArgumentException("Tried to switch the game to a state that doesn't exist or is not known.");
            }
        }
    }
}