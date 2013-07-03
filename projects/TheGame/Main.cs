using System.Diagnostics;
using Examples.TheGame.Networking;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    ///     This class contains the main entry point of the game
    /// </summary>
    public class TheGame : RenderCanvas
    {
        private static float _red, _green, _blue;
        protected ShaderMaterial M;
        protected IShaderParam[] Param;
        protected ShaderProgram Sp;

        /// <summary>
        ///     The main game handler
        /// </summary>
        private GameHandler _gameHandler;

        private NetworkHandler _networkHandler;

        private Mediator _mediator;

        /// <summary>
        ///     Initialize FUSEE
        /// </summary>
        public override void Init()
        {
            Sp = MoreShaders.GetShader("diffuse", RC);
            RC.SetShader(Sp);
            _red = _green = _blue = 0.1f;
            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(500, 0, 0));
            RC.SetLightAmbient(0, new float4(0.3f, 0.3f, 0.3f, 1));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1));
            RC.SetLightDiffuse(0, new float4(_red, _green, _blue, 1));
            RC.SetLightDirection(0, new float3(-1, 0, 0));


            M = new ShaderMaterial(Sp);

            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);

            
            _mediator = new Mediator();

            _gameHandler = new GameHandler(RC, _mediator);
            _networkHandler = new NetworkHandler(RC, _mediator);
        }

        /// <summary>
        ///     Renders everything
        /// </summary>
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Update game handler and render it
            // _gameHandler.UpdateStates();
            // _gameHandler.RenderAFrame();

            _networkHandler.HandleNetwork();
            Debug.WriteLine(_mediator.GetObjectId());

            Present();
        }

        /// <summary>
        ///     Resizes the game window.
        /// </summary>
        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        /// <summary>
        ///     Main entry point of the game
        /// </summary>
        public static void Main()
        {
            var app = new TheGame();
            app.Run();
        }
    }
}