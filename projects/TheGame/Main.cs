﻿using System.IO;
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
        public struct SpotLight
        {
            public IShaderParam Position;
            public IShaderParam Direction;
            public IShaderParam Diffuse;
            public IShaderParam Specular;
            public IShaderParam Aperture;
            public IShaderParam Falloff;
        };
        private static float _red, _green, _blue;
        protected ShaderMaterial M;
        protected Mesh Mesh;
        protected SpotLight[] LightShaderParams = new SpotLight[8];
        protected IShaderParam CalcLightingShaderParam;
        protected IShaderParam AmbientLightShaderParam;
        protected IShaderParam MaterialAmbientShaderParam;
        protected IShaderParam MaterialDiffuseShaderParam;
        protected IShaderParam MaterialSpecularShaderParam;
        protected IShaderParam MaterialShininessShaderParam;
        protected IShaderParam CamPositionShaderParam;
        protected IShaderParam AmountOfLightsShaderParam;
        protected IShaderParam NoiseStrengthShaderParam;
        protected IShaderParam NoiseTimeShaderParam;
        protected IShaderParam NoiseOffsetShaderParam;
        protected ShaderProgram Sp;

        // Shader stuff
        protected ImageData Texture;
        protected ITexture TextureHandle;
        protected IShaderParam TextureShaderParam;

        private float3 _cameraPos = new float3(0, 1000, -1000);
        private float _light1Falloff = 10000;
        private float _light1Aperture = 0.1f;
        private float _noiseTime = 0.0f;

        private float _yRot = 0;

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
            CamPositionShaderParam = Sp.GetShaderParam("camPosition");
            AmountOfLightsShaderParam = Sp.GetShaderParam("amountOfLights");
            for(int i = 1; i < 9; i++)
            {
                LightShaderParams[i-1].Position = Sp.GetShaderParam("light" + i + "Position");
                LightShaderParams[i-1].Direction = Sp.GetShaderParam("light" + i + "Direction");
                LightShaderParams[i-1].Diffuse = Sp.GetShaderParam("light" + i + "Diffuse");
                LightShaderParams[i-1].Specular = Sp.GetShaderParam("light" + i + "Specular");
                LightShaderParams[i-1].Aperture = Sp.GetShaderParam("light" + i + "Aperture");
                LightShaderParams[i-1].Falloff = Sp.GetShaderParam("light" + i + "Falloff");
            }
            NoiseStrengthShaderParam = Sp.GetShaderParam("noiseStrength");
            NoiseTimeShaderParam = Sp.GetShaderParam("noiseTime");
            NoiseOffsetShaderParam = Sp.GetShaderParam("noiseOffset");

            // Load texture
            Texture = RC.LoadImage("Assets/SpaceShip_Diffuse.jpg");
            TextureHandle = RC.CreateTexture(Texture);

            // Init shader
            RC.SetShaderParam(CalcLightingShaderParam, 0);
            RC.SetShaderParam(AmbientLightShaderParam, new float4(0.0f, 0.0f, 0.0f, 1.0f));
            RC.SetShaderParam(MaterialAmbientShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetShaderParam(MaterialDiffuseShaderParam, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(MaterialSpecularShaderParam, new float4(0.1f, 0.1f, 0.2f, 1.0f));
            //RC.SetShaderParam(MaterialShininessShaderParam, 5.0f);

            //RC.SetShaderParam(CamPositionShaderParam, new float4(_cameraPos.x, _cameraPos.y, _cameraPos.z, 0.0f));

            RC.SetShaderParam(AmountOfLightsShaderParam, 1);

            //RC.SetShaderParam(LightShaderParams[0].Position, new float4(1000, 1000, -1000, 1));
            RC.SetShaderParam(LightShaderParams[0].Direction, new float3(-1, -1, 1));
            RC.SetShaderParam(LightShaderParams[0].Diffuse, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(LightShaderParams[0].Specular, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(LightShaderParams[0].Aperture, _light1Aperture);
            //RC.SetShaderParam(LightShaderParams[0].Falloff, _light1Falloff);

            RC.SetShaderParam(NoiseStrengthShaderParam, 0.5f);
            RC.SetShaderParam(NoiseTimeShaderParam, 0.0f);
            RC.SetShaderParam(NoiseOffsetShaderParam, new float2(0, 0));
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

            if (Input.Instance.IsKeyDown(KeyCodes.End))
                _light1Aperture += 0.1f * (float)Time.Instance.DeltaTime;
            if (Input.Instance.IsKeyDown(KeyCodes.Home))
                _light1Aperture -= 0.1f * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Add))
            {
                _noiseTime += 0.1f * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Subtract))
            {
                _noiseTime -= 0.1f * (float)Time.Instance.DeltaTime;
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Q))
            {
                _yRot -= 0.5f * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.E))
            {
                _yRot += 0.5f * (float)Time.Instance.DeltaTime;
            }


            float4x4 mtxRot = float4x4.CreateRotationZ(0) * float4x4.CreateRotationY(_yRot) * float4x4.CreateRotationX(0);
            float4x4 mtxCam = float4x4.LookAt(_cameraPos, new float3(0, 0, 0), new float3(0, 1, 0));
            float4x4 mtxScale = float4x4.Scale(1f, 1f, 1f);
            float4x4 mtxPos = float4x4.CreateTranslation(0, 0, 0);

            RC.ModelView = mtxScale * mtxRot * mtxPos * mtxCam;

            RC.SetShaderParam(LightShaderParams[0].Position, RC.View * new float4(1000, 1000, -1000, 1));
            RC.SetShaderParam(LightShaderParams[0].Direction, RC.InvTransView * new float3(1, -1, -1));
            RC.SetShaderParam(LightShaderParams[0].Diffuse, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(LightShaderParams[0].Specular, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            //RC.SetShaderParam(LightShaderParams[0].Aperture, _light1Aperture);
            RC.SetShaderParam(LightShaderParams[0].Falloff, _light1Falloff);

            //RC.SetShaderParam(CamPositionShaderParam, new float4(_cameraPos.x, _cameraPos.y, _cameraPos.z, 0.0f));
            RC.SetShaderParamTexture(TextureShaderParam, TextureHandle);
            RC.SetShaderParam(NoiseTimeShaderParam, _noiseTime);

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