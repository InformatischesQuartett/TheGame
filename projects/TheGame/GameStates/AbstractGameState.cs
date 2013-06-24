using System.Collections.Generic;
using Examples.TheGame.GameEntity;
using Fusee.Engine;

namespace Examples.TheGame.GameStates
{
    /// <summary>
    ///     Abstract game state class. Implements some basic functionality every game state has and defines a common interface.
    /// </summary>
    public abstract class AbstractGameState
    {
        /// <summary>
        ///     A list containing all game entities that are maintained by this state
        /// </summary>
        private readonly List<AbstractGameEntity> _gameEntities = new List<AbstractGameEntity>();

        /// <summary>
        ///     A list containing all game entities that are maintained by this state
        /// </summary>
        public List<AbstractGameEntity> GameEntities
        {
            get { return _gameEntities; }
        }

        /// <summary>
        ///     Gets the ID of this state
        /// </summary>
        /// <returns>The ID of this game state</returns>
        public abstract GameState GetId();

        /// <summary>
        ///     Updates this game state.
        /// </summary>
        public virtual void Update()
        {
            // Automatically update all entities in this state
            foreach (AbstractGameEntity entity in GameEntities)
            {
                entity.Update(this);
            }
        }

        /// <summary>
        ///     Renders the game state using the specified render context
        /// </summary>
        /// <param name="rc">The render context to use</param>
        public virtual void Render(RenderContext rc)
        {
            // Automatically render all entities in this state
            foreach (AbstractGameEntity entity in GameEntities)
            {
                entity.Render(rc, this);
            }
        }
    }
}