using System.Collections.Generic;
using Examples.TheGame.Entities;
using Examples.TheGame.Networking;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    internal class GameHandler
    {
        /// <summary>
        ///Disctionarires mit allen Items und Playern
        /// </summary>
        public static Dictionary<int, HealthItem> HealthItems;
        public static Dictionary<int, Bullet> Bullets;
        public static Dictionary<int, Player> Players;

        /// <summary>
        ///State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        private readonly Mediator _mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        private Player p;
    
        private Mesh TheMesh;
        internal GameHandler(RenderContext rc, Mediator mediator)
        {
            //pass RenderContext
            _rc = rc;
            _mediator = mediator;

            HealthItems = new Dictionary<int, HealthItem>();
            Bullets = new Dictionary<int, Bullet>();
            Players = new Dictionary<int, Player>();

            GameState = new GameState(GameState.State.StartMenu);
            p = new Player(_mediator, _rc, 1, float4x4.Identity, 0, 0);
          
            
            this.TheMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");

        }

        internal void Update()
        {
            p.Update();
            foreach (var go in HealthItems)
                go.Value.Update();
            foreach (var go in Bullets)
                go.Value.Update();
            foreach (var go in Players)
                go.Value.Update();
            
        }

        internal void Render()
        {
            //p.RenderUpdate(_rc);


            //rendern
            ShaderProgram sp = MoreShaders.GetShader("simple", _rc);
            _rc.SetShader(sp);
            IShaderParam _vColorParam = sp.GetShaderParam("vColor");
            _rc.SetShaderParam(_vColorParam, new float4(1.0f, 0.5f, 0.5f, 1));
            _rc.ModelView = float4x4.Identity;
            _rc.Render(TheMesh);

            foreach (var go in HealthItems)
                go.Value.RenderUpdate(_rc);
            foreach (var go in Bullets)
                go.Value.RenderUpdate(_rc);
            foreach (var go in Players)
                go.Value.RenderUpdate(_rc);
        }
    }
}