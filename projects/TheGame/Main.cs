using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    ///     This class contains the main entry point of the game
    /// </summary>
    public class TheGame : RenderCanvas
    {
        private Mediator _mediator;

        /// <summary>
        ///     Initialize FUSEE
        /// </summary>
        public override void Init()
        {
            // Settings for the GameWindow
            Blending = true;
            VSync = true;

            Height = 700;
            Width = 750;

            Input.Instance.CursorVisible = false;
            Input.Instance.FixMouseAtCenter = true;

            RC.ClearColor = new float4(0f, 0f, 0f, 1);

            // Mediator for GameHandler and NetworkHandler
            const bool networkActive = true;
            _mediator = new Mediator(RC, networkActive);
        }

        /// <summary>
        ///     Renders everything
        /// </summary>
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            Blending = _mediator.Blending;
            Fullscreen = _mediator.Fullscreen;

            _mediator.Update();

            Present();
        }

        /// <summary>
        ///     Resizes the game window.
        /// </summary>
        public override void Resize()
        {
            // is called when the window is resized
            _mediator.Width = Width;
            _mediator.Height = Height;

            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1000000);
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