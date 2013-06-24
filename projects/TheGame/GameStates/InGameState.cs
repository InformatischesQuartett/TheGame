using Fusee.Engine;

namespace Examples.TheGame.GameStates
{
    /// <summary>
    ///     The game state used when the player currently is in the game
    /// </summary>
    internal class InGameState : AbstractGameState
    {
        /// <summary>
        ///     Gets the ID of this state
        /// </summary>
        /// <returns>
        ///     The ID of this game state
        /// </returns>
        public override GameState GetId()
        {
            return GameState.InGame;
        }

        /// <summary>
        ///     Updates this game state.
        /// </summary>
        public override void Update()
        {
            base.Update();
            // TODO: Update the state here
        }

        public override void Render(RenderContext rc)
        {
            base.Render(rc);
            // TODO: Render anything over here
        }
    }
}