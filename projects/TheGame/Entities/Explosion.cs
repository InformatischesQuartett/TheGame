using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    /// Explosion entity. It... explodes.
    /// </summary>
    internal class Explosion : GameEntity
    {
        /// <summary>
        /// The elapsed time since start of the explosion
        /// </summary>
        private double _elapsedTime = 0;

        /// <summary>
        /// The max time for the explosion to last in milliseconds
        /// </summary>
        private const int MaxTime = 10;

        /// <summary>
        /// The size increase of the explosion per time step
        /// </summary>
        private const float SizeIncrease = 1f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Explosion"/> class.
        /// </summary>
        /// <param name="gameHandler">The GameHandler</param>
        /// <param name="position">The position.</param>
        internal Explosion(GameHandler gameHandler, float4x4 position) : base(gameHandler, 0, position, 0, 0)
        {
            this.EntityMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        internal override void Update()
        {
            base.Update();

            _elapsedTime += Time.Instance.DeltaTime;

            // Scale up or down
            if (_elapsedTime <= MaxTime/2)
            {
                SetScale(GetScale() + (float) (SizeIncrease * Time.Instance.DeltaTime));
                Debug.WriteLine("Explosion increase: " + GetScale());
            }
            else if (_elapsedTime > MaxTime / 2)
            {
                SetScale(GetScale() - (float)(SizeIncrease * Time.Instance.DeltaTime));
                Debug.WriteLine("Explosion decrease: " + GetScale());
            }
            if (_elapsedTime >= MaxTime)
            {
                Debug.WriteLine("Explosion destroyed");

                this.DestroyEnity();
            }
        }
    }
}