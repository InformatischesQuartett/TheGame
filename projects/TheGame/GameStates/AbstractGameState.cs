using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace Examples.TheGame.GameStates
{
    // TODO: Add game entities that are automatically handled by game states

    /// <summary>
    /// Abstract game state class. Implements some basic functionality every game state has and defines a common interface.
    /// </summary>
    public abstract class AbstractGameState
    {
        /// <summary>
        /// Gets the ID of this state
        /// </summary>
        /// <returns>The ID of this game state</returns>
        public abstract GameState GetId();

        /// <summary>
        /// Updates this game state.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Renders the game state using the specified render context
        /// </summary>
        /// <param name="rc">The render context to use</param>
        public abstract void Render(RenderContext rc);
    }
}