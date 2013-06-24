using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;

namespace Examples.TheGame.GameStates
{
    /// <summary>
    /// The game state used when the player currently is in the game
    /// </summary>
    class InGameState : AbstractGameState
    {
        /// <summary>
        /// Gets the ID of this state
        /// </summary>
        /// <returns>
        /// The ID of this game state
        /// </returns>
        public override GameState GetId()
        {
            return GameState.InGame;
        }

        /// <summary>
        /// Updates this game state.
        /// </summary>
        public override void Update()
        {
            // TODO: Update the state here
        }

        public override void Render(RenderContext rc)
        {
            // TODO: Render anything over here
        }
    }
}
