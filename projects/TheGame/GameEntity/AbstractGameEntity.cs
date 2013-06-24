using Examples.TheGame.GameStates;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.GameEntity
{
    /// <summary>
    ///     Abstract game entity which is the base for all other entities in the game. It defines the general interface for all entites and some basic functionality.
    /// </summary>
    public abstract class AbstractGameEntity
    {
        /// <summary>
        ///     The position in 3D space of this entity
        /// </summary>
        /// <value>
        ///     The position of the game entity in 3D space.
        /// </value>
        public float3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the speed of this entity.
        /// </summary>
        /// <value>
        ///     The speed of the entity as a 3D vector.
        /// </value>
        public float3 Speed { get; set; }

        /// <summary>
        ///     Gets or sets the rotation of the entity.
        /// </summary>
        /// <value>
        ///     The rotation of the entity.
        /// </value>
        public Quaternion Rotation { get; set; }

        /// <summary>
        ///     Updates this entity
        /// </summary>
        /// <param name="gameState">Game state that issued the update (i.e. this entity belongs to).</param>
        public abstract void Update(AbstractGameState gameState);

        /// <summary>
        ///     Renders the game entity using the specified render context
        /// </summary>
        /// <param name="rc">The render context to use</param>
        /// ///
        /// <param name="gameState">Game state that issued the render call (i.e. this entity belongs to).</param>
        public abstract void Render(RenderContext rc, AbstractGameState gameState);
    }
}