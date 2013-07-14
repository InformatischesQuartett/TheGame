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
        private const float MaxTime = 1.5f;

        /// <summary>
        /// The size increase of the explosion per time step
        /// </summary>
        private const float SizeIncrease = 600f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Explosion"/> class.
        /// </summary>
        /// <param name="gameHandler">The GameHandler</param>
        /// <param name="position">The position.</param>
        internal Explosion(GameHandler gameHandler, float4x4 position) : base(gameHandler, 0, position, 0, 0)
        {
            this.EntityMesh = MeshReader.LoadMesh("Assets/Sphere.obj.model");
            SetScale(100);
            _sp = gameHandler.CustomSp;
            SetId(gameHandler.Mediator.GetObjectId());
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        internal override void Update()
        {
            base.Update();

            _elapsedTime += Time.Instance.DeltaTime;

            // Scale up or down
            if (_elapsedTime <= MaxTime/2.0f)
            {
                SetScale(GetScale() + (float) (SizeIncrease * Time.Instance.DeltaTime));
                Debug.WriteLine("Explosion increase: " + GetScale());
            }
            else if (_elapsedTime > MaxTime / 2.0f)
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

        /// <summary>
        /// Instructs the shader prior to rendering
        /// </summary>
        internal override void InstructShader()
        {
            _rc.SetShaderParamTexture(_sp.GetShaderParam("tex"), _gameHandler.TextureExplosionHandle);
            _rc.SetShaderParam(_sp.GetShaderParam("calcLighting"), 1);
            _rc.SetShaderParam(_sp.GetShaderParam("ambientLight"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            _rc.SetShaderParam(_sp.GetShaderParam("matAmbient"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            _rc.SetShaderParam(_sp.GetShaderParam("noiseStrength"), 1.0f);
            _rc.SetShaderParam(_sp.GetShaderParam("noiseTime"), (float)_elapsedTime/5);
            _rc.SetShaderParam(_sp.GetShaderParam("noiseOffset"), new float2(0, 0));
        }
    }
}