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
        private float SizeIncrease = 600f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Explosion"/> class.
        /// </summary>
        /// <param name="gameHandler">The GameHandler</param>
        /// <param name="position">The position.</param>
        internal Explosion(GameHandler gameHandler, float4x4 position) : base(gameHandler, position, 0)
        {
            EntityMesh = gameHandler.ExplosionMesh;
            SetScale(100);
            Sp = gameHandler.CustomSp;
            SetId(gameHandler.Mediator.GetObjectId());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Explosion"/> class.
        /// </summary>
        /// <param name="gameHandler">The GameHandler</param>
        /// <param name="position">The position.</param>
        /// <param name="sizeIncrease">Increase in size of the explosion</param>
        internal Explosion(GameHandler gameHandler, float4x4 position, float sizeIncrease)
            : base(gameHandler, position, 0)
        {
            EntityMesh = MeshReader.LoadMesh("Assets/Sphere.obj.model");
            SetScale(100);
            Sp = gameHandler.CustomSp;
            SetId(gameHandler.Mediator.GetObjectId());
            SizeIncrease = sizeIncrease;
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
                SetScale(GetScale() + (float) (SizeIncrease * Time.Instance.DeltaTime));

            else if (_elapsedTime > MaxTime / 2.0f)
                SetScale(GetScale() - (float)(SizeIncrease * Time.Instance.DeltaTime));

            if (_elapsedTime >= MaxTime)
                DestroyEnity();
        }

        /// <summary>
        /// Instructs the shader prior to rendering
        /// </summary>
        internal override void InstructShader()
        {
            Rc.SetShaderParamTexture(Sp.GetShaderParam("tex"), GameHandler.TextureExplosionHandle);
            Rc.SetShaderParam(Sp.GetShaderParam("calcLighting"), 1);
            Rc.SetShaderParam(Sp.GetShaderParam("ambientLight"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("matAmbient"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("noiseStrength"), 1.5f);
            Rc.SetShaderParam(Sp.GetShaderParam("noiseTime"), (float)_elapsedTime/5);
            Rc.SetShaderParam(Sp.GetShaderParam("noiseOffset"), new float2(0, 0));
        }
    }
}