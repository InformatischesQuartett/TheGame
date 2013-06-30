using System.IO;
using Examples.TheGame.Shader;
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
        protected Mesh Mesh;
        protected IShaderParam[] Param;
        protected IShaderParam ReceiveShadowsShaderParam;
        protected IShaderParam LightDirShaderParam;
        protected IShaderParam LightColorShaderParam;
        protected IShaderParam AmbientLightShaderParam;
        protected ShaderProgram Sp;

        // Shader stuff
        protected ImageData Texture;
        protected ITexture TextureHandle;
        protected IShaderParam TextureShaderParam;

        /// <summary>
        ///     The main game handler
        /// </summary>
        private GameHandler _gameHandler;

        /// <summary>
        ///     Initialize FUSEE
        /// </summary>
        public override void Init()
        {
            Sp = RC.CreateShader(ShaderCode.GetVertexShader(), ShaderCode.GetFragmentShader());
            RC.SetShader(Sp);


            M = new ShaderMaterial(Sp);

            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);

            // Create game handler
            _gameHandler = new GameHandler(RC);


            // Load mesh
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader("Assets/SpaceShip.obj.model"));
            Mesh = geo.ToMesh();

            // Create shader and store handle
            TextureShaderParam = Sp.GetShaderParam("texture1");
            ReceiveShadowsShaderParam = Sp.GetShaderParam("receiveShadows");

            // Load texture
            Texture = RC.LoadImage("Assets/SpaceShip_Diffuse.jpg");
            TextureHandle = RC.CreateTexture(Texture);

            // Init shader
            LightDirShaderParam = Sp.GetShaderParam("lightDir");
            LightColorShaderParam = Sp.GetShaderParam("lightColor");
            AmbientLightShaderParam = Sp.GetShaderParam("ambientLight");
            ReceiveShadowsShaderParam = Sp.GetShaderParam("receiveShadows");
            // Hardcoded lights, I should feel really bad
            RC.SetShaderParam(LightDirShaderParam, new float3(-1, 0.5f, 0));
            RC.SetShaderParam(LightColorShaderParam, new float4(0.8f, 0.8f, 1, 1));
            RC.SetShaderParam(AmbientLightShaderParam, new float4(0.3f, 0.3f, 0.4f, 1));
            RC.SetShaderParam(ReceiveShadowsShaderParam, 1);
        }

        /// <summary>
        ///     Renders everything
        /// </summary>
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Update game handler and render it
            //    _gameHandler.UpdateStates();
            //   _gameHandler.RenderAFrame();

            float4x4 mtxRot = float4x4.CreateRotationZ(0) * float4x4.CreateRotationY(45) * float4x4.CreateRotationX(45);
            float4x4 mtxCam = float4x4.Identity;
            float4x4 mtxScale = float4x4.Scale(1, 1, 1);
            float4x4 mtxPos = float4x4.CreateTranslation(0, 0, -1000);

            RC.ModelView = mtxScale * mtxRot * mtxPos * mtxCam;

            RC.SetShaderParamTexture(TextureShaderParam, TextureHandle);
            RC.Render(Mesh);
            RC.SetShaderParam(ReceiveShadowsShaderParam, 1);

            Present();
        }

        /// <summary>
        ///     Resizes the game window.
        /// </summary>
        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width/(float) Height;
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