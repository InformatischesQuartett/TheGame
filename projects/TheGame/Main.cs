using System.IO;
using System.Collections.Generic;

using Fusee.Engine;
using Fusee.Math;

using Examples.TheGame.GameStates;

namespace Examples.TheGame
{
    public class TdM : RenderCanvas
    {


        protected ShaderProgram Sp;
        protected IShaderParam[] Param;
        protected ShaderMaterial M;

        private static float _red, _green, _blue;

        //GameHandler 
        private GameHandler _gameHandler;

        public override void Init()
        {
            Sp = MoreShaders.GetShader("diffuse", RC);
            RC.SetShader(Sp);
            _red = _green = _blue = 0.1f;
            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(500, 0, 0));
            RC.SetLightAmbient(0, new float4(0.3f, 0.3f, 0.3f, 1));
            RC.SetLightDiffuse(0, new float4(_red, _green, _blue, 1));
            RC.SetLightDirection(0, new float3(-1, 0, 0));


            M = new ShaderMaterial(Sp);

            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);
            // is called on startup

            //initialize new GmaeHandler object
            _gameHandler = new GameHandler(RC);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
         
            _gameHandler.RenderAFrame();

            Present();
        }
            
        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new TdM();
            app.Run();
        }

    }
}
