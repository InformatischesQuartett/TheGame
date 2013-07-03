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
        protected IShaderParam CalcLightingShaderParam;
        protected IShaderParam AmbientLightShaderParam;
        protected IShaderParam MaterialAmbientShaderParam;
        protected IShaderParam MaterialDiffuseShaderParam;
        protected IShaderParam MaterialSpecularShaderParam;
        protected IShaderParam MaterialShininessShaderParam;
        protected IShaderParam AmountOfLightsShaderParam;
        protected IShaderParam Light1PositionShaderParam;
        protected IShaderParam Light1DirectionShaderParam;
        protected IShaderParam Light1DiffuseShaderParam;
        protected IShaderParam Light1SpecularShaderParam;
        protected IShaderParam Light1FalloffShaderParam;
        protected IShaderParam Light2PositionShaderParam;
        protected IShaderParam Light2DirectionShaderParam;
        protected IShaderParam Light2DiffuseShaderParam;
        protected IShaderParam Light2SpecularShaderParam;
        protected IShaderParam Light2FalloffShaderParam;
        protected IShaderParam Light3PositionShaderParam;
        protected IShaderParam Light3DirectionShaderParam;
        protected IShaderParam Light3DiffuseShaderParam;
        protected IShaderParam Light3SpecularShaderParam;
        protected IShaderParam Light3FalloffShaderParam;
        protected IShaderParam Light4PositionShaderParam;
        protected IShaderParam Light4DirectionShaderParam;
        protected IShaderParam Light4DiffuseShaderParam;
        protected IShaderParam Light4SpecularShaderParam;
        protected IShaderParam Light4FalloffShaderParam;
        protected IShaderParam Light5PositionShaderParam;
        protected IShaderParam Light5DirectionShaderParam;
        protected IShaderParam Light5DiffuseShaderParam;
        protected IShaderParam Light5SpecularShaderParam;
        protected IShaderParam Light5FalloffShaderParam;
        protected IShaderParam Light6PositionShaderParam;
        protected IShaderParam Light6DirectionShaderParam;
        protected IShaderParam Light6DiffuseShaderParam;
        protected IShaderParam Light6SpecularShaderParam;
        protected IShaderParam Light6FalloffShaderParam;
        protected IShaderParam Light7PositionShaderParam;
        protected IShaderParam Light7DirectionShaderParam;
        protected IShaderParam Light7DiffuseShaderParam;
        protected IShaderParam Light7SpecularShaderParam;
        protected IShaderParam Light7FalloffShaderParam;
        protected IShaderParam Light8PositionShaderParam;
        protected IShaderParam Light8DirectionShaderParam;
        protected IShaderParam Light8DiffuseShaderParam;
        protected IShaderParam Light8SpecularShaderParam;
        protected IShaderParam Light8FalloffShaderParam;
        protected ShaderProgram Sp;

        // Shader stuff
        protected ImageData Texture;
        protected ITexture TextureHandle;
        protected IShaderParam TextureShaderParam;

        private float3 _cameraPos = new float3(0, 0, 1000);
        private float _light1Falloff = 10000;

        /// <summary>
        ///     The main game handler
        /// </summary>
        private GameHandler _gameHandler;

        /// <summary>
        /// Initialize FUSEE
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
            TextureShaderParam = Sp.GetShaderParam("tex");
            CalcLightingShaderParam = Sp.GetShaderParam("calcLighting");
            AmbientLightShaderParam = Sp.GetShaderParam("ambientLight");
            MaterialAmbientShaderParam = Sp.GetShaderParam("matAmbient");
            MaterialDiffuseShaderParam = Sp.GetShaderParam("matDiffuse");
            MaterialSpecularShaderParam = Sp.GetShaderParam("matSpecular");
            MaterialShininessShaderParam = Sp.GetShaderParam("matShininess");
            AmountOfLightsShaderParam = Sp.GetShaderParam("amountOfLights");
            Light1PositionShaderParam = Sp.GetShaderParam("light1Position");
            Light1DirectionShaderParam = Sp.GetShaderParam("light1Direction");
            Light1DiffuseShaderParam = Sp.GetShaderParam("light1Diffuse");
            Light1SpecularShaderParam = Sp.GetShaderParam("light1Specular");
            Light1FalloffShaderParam = Sp.GetShaderParam("light1Falloff");
            Light2PositionShaderParam = Sp.GetShaderParam("light2Position");
            Light2DirectionShaderParam = Sp.GetShaderParam("light2Direction");
            Light2DiffuseShaderParam = Sp.GetShaderParam("light2Diffuse");
            Light2SpecularShaderParam = Sp.GetShaderParam("light2Specular");
            Light2FalloffShaderParam = Sp.GetShaderParam("light2Falloff");
            Light3PositionShaderParam = Sp.GetShaderParam("light3Position");
            Light3DirectionShaderParam = Sp.GetShaderParam("light3Direction");
            Light3DiffuseShaderParam = Sp.GetShaderParam("light3Diffuse");
            Light3SpecularShaderParam = Sp.GetShaderParam("light3Specular");
            Light3FalloffShaderParam = Sp.GetShaderParam("light3Falloff");
            Light4PositionShaderParam = Sp.GetShaderParam("light4Position");
            Light4DirectionShaderParam = Sp.GetShaderParam("light4Direction");
            Light4DiffuseShaderParam = Sp.GetShaderParam("light4Diffuse");
            Light4SpecularShaderParam = Sp.GetShaderParam("light4Specular");
            Light4FalloffShaderParam = Sp.GetShaderParam("light4Falloff");
            Light5PositionShaderParam = Sp.GetShaderParam("light5Position");
            Light5DirectionShaderParam = Sp.GetShaderParam("light5Direction");
            Light5DiffuseShaderParam = Sp.GetShaderParam("light5Diffuse");
            Light5SpecularShaderParam = Sp.GetShaderParam("light5Specular");
            Light5FalloffShaderParam = Sp.GetShaderParam("light5Falloff");
            Light6PositionShaderParam = Sp.GetShaderParam("light6Position");
            Light6DirectionShaderParam = Sp.GetShaderParam("light6Direction");
            Light6DiffuseShaderParam = Sp.GetShaderParam("light6Diffuse");
            Light6SpecularShaderParam = Sp.GetShaderParam("light6Specular");
            Light6FalloffShaderParam = Sp.GetShaderParam("light6Falloff");
            Light7PositionShaderParam = Sp.GetShaderParam("light7Position");
            Light7DirectionShaderParam = Sp.GetShaderParam("light7Direction");
            Light7DiffuseShaderParam = Sp.GetShaderParam("light7Diffuse");
            Light7SpecularShaderParam = Sp.GetShaderParam("light7Specular");
            Light7FalloffShaderParam = Sp.GetShaderParam("light7Falloff");
            Light8PositionShaderParam = Sp.GetShaderParam("light8Position");
            Light8DirectionShaderParam = Sp.GetShaderParam("light8Direction");
            Light8DiffuseShaderParam = Sp.GetShaderParam("light8Diffuse");
            Light8SpecularShaderParam = Sp.GetShaderParam("light8Specular");
            Light8FalloffShaderParam = Sp.GetShaderParam("light8Falloff");

            // Load texture
            Texture = RC.LoadImage("Assets/SpaceShip_Diffuse.jpg");
            TextureHandle = RC.CreateTexture(Texture);

            // Init shader
            RC.SetShaderParam(CalcLightingShaderParam, 0);
            RC.SetShaderParam(AmbientLightShaderParam, new float4(0.0f, 0.0f, 0.0f, 1.0f));
            RC.SetShaderParam(MaterialAmbientShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetShaderParam(MaterialDiffuseShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(MaterialSpecularShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(MaterialShininessShaderParam, 1.0f);

            RC.SetShaderParam(AmountOfLightsShaderParam, 1);

            RC.SetShaderParam(Light1PositionShaderParam, new float4(-1000, 0, -1000, 1));
            RC.SetShaderParam(Light1DirectionShaderParam, new float3(0, 1, 1));
            RC.SetShaderParam(Light1DiffuseShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetShaderParam(Light1SpecularShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetShaderParam(Light1FalloffShaderParam, _light1Falloff);

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

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _cameraPos.x -= 500 * (float)Time.Instance.DeltaTime;
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _cameraPos.x += 500 * (float)Time.Instance.DeltaTime;
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _cameraPos.z -= 500 * (float)Time.Instance.DeltaTime;
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _cameraPos.z += 500 * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.PageDown))
                _light1Falloff -= 5000 * (float)Time.Instance.DeltaTime;
            if (Input.Instance.IsKeyDown(KeyCodes.PageUp))
                _light1Falloff += 5000 * (float)Time.Instance.DeltaTime;


            float4x4 mtxRot = float4x4.CreateRotationZ(0) * float4x4.CreateRotationY(45) * float4x4.CreateRotationX(45);
            float4x4 mtxCam = float4x4.LookAt(_cameraPos, new float3(0, 0, 0), new float3(0, 1, 0));
            float4x4 mtxScale = float4x4.Scale(1f, 1f, 1f);
            float4x4 mtxPos = float4x4.CreateTranslation(0, 0, 0);

            RC.ModelView = mtxScale * mtxRot * mtxPos * mtxCam;

            RC.SetShaderParam(Light1FalloffShaderParam, _light1Falloff);
            RC.SetShaderParamTexture(TextureShaderParam, TextureHandle);
            RC.Render(Mesh);

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