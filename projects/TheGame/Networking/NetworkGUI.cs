using System;
using System.Diagnostics;
using System.Globalization;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class NetworkGUI
    {
        private readonly NetworkHandler _networkHandler;
        private readonly RenderContext _renderContext;

        private NetworkServer _networkServer;
        private NetworkClient _networkClient;

        private readonly Mesh _guiPlaneMesh;
        private float xPos;

        private readonly ShaderProgram _guiShader;
        private readonly IShaderParam _texParam;

        private readonly ImageData _emptyBg;
        private readonly ITexture[] _textures;

        private ITexture _guiTex;
        private string _lastMsg;
        private int _chosenEntry;

        private readonly IAudioStream _menuSound;
        private readonly IAudioStream _menuSound2;

        internal string ConnectToIp = "Discovery?";

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkGUI" /> class.
        /// </summary>
        /// <param name="renderContext">Reference to the RenderContext</param>
        /// <param name="networkHandler">Reference to the NetworkHandler</param>
        internal NetworkGUI(RenderContext renderContext, NetworkHandler networkHandler)
        {
            _renderContext = renderContext;
            _networkHandler = networkHandler;

            // load GUIMesh
            _guiPlaneMesh = MeshReader.LoadMesh("Assets/guiPlane.obj.model");
            xPos = 500;

            // load Texture
            _guiShader = MoreShaders.GetShader("texture", _renderContext);
            _texParam = _guiShader.GetShaderParam("texture1");
            _textures = new ITexture[4];

            var imgData = _renderContext.LoadImage("Assets/menue_client.png");
            _textures[0] = _renderContext.CreateTexture(imgData);

            imgData = _renderContext.LoadImage("Assets/menue_server.png");
            _textures[1] = _renderContext.CreateTexture(imgData);

            imgData = _renderContext.LoadImage("Assets/menue_close.png");
            _textures[2] = _renderContext.CreateTexture(imgData);

            imgData = _renderContext.LoadImage("Assets/guiSpace.jpg");
            _textures[3] = _renderContext.CreateTexture(imgData);

            _emptyBg = _renderContext.LoadImage("Assets/menue_empty.png");

            // load Sound
            _menuSound = Audio.Instance.LoadFile("Assets/MenuBeep.wav");
            _menuSound2 = Audio.Instance.LoadFile("Assets/MenuBeep2.wav");

            _menuSound.Volume = 75f;
            _menuSound2.Volume = 75f;

            _chosenEntry = 0;
            RefreshGUITex();
        }

        /// <summary>
        ///     Refreshes the GUI.
        /// </summary>
        internal void RefreshGUITex()
        {
            if (Network.Instance.Config.SysType == SysType.None)
            {
                _guiTex = _textures[_chosenEntry];
            }
            else
            {
                string msg = "";

                if (Network.Instance.Config.SysType == SysType.Server)
                    msg += "IP of the Server:\n\n              " + Network.Instance.LocalIP + "\n\nPlayers: " +
                           Network.Instance.Connections.Count + "             (Press Space)";

                if (Network.Instance.Config.SysType == SysType.Client)
                    if (Network.Instance.Status.Connected)
                        msg += "Connected to:\n\n       " + ConnectToIp + "\n\nWaiting...";
                    else if (Network.Instance.Status.Connecting)
                        msg += "Connecting...";
                    else
                        msg += "IP of the Server:\n\n              " + ConnectToIp + "\n\n\t               (Press Return)";

                if (msg != _lastMsg)
                {
                    var finalImg = _renderContext.TextOnImage(_emptyBg, "Calibri", 56, msg, "white", 60, 60);
                    _guiTex = _renderContext.CreateTexture(finalImg);
                }

                _lastMsg = msg;
            }
        }

        /// <summary>
        ///     Displays the GUI for network settings.
        /// </summary>
        internal void ShowNetworkGUI()
        {
            RefreshGUITex();

            // Change ViewPort and aspectRatio (fullsize)
            _renderContext.Viewport(0, 0, _networkHandler.Mediator.Width, _networkHandler.Mediator.Height);

            var aspectRatio = _networkHandler.Mediator.Width/_networkHandler.Mediator.Height;
            _renderContext.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
            
            // Set Shader and ModelView
            xPos = (xPos < -500) ? 500 : xPos - (float) Time.Instance.DeltaTime*0.75f;

            _renderContext.SetShaderParamTexture(_texParam, _textures[3]);
            _renderContext.ModelView = float4x4.Scale(new float3(0.5f, 0.25f, 1))*float4x4.CreateTranslation(xPos, 0, 0)*
                                       float4x4.LookAt(0, 0, 400, 0, 0, 0, 0, 1, 0);

            _renderContext.Render(_guiPlaneMesh);

            // Change ViewPort and aspectRatio (GUI size)
            _renderContext.Viewport(_networkHandler.Mediator.Width/2 - 848/2,
                                    _networkHandler.Mediator.Height/2 - 436/2,
                                    848, 436);

            _renderContext.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 848/436, 1, 10000);

            // Set Shader and ModelView
            _renderContext.Clear(ClearFlags.Depth);
            _renderContext.SetShader(_guiShader);
            _renderContext.SetShaderParamTexture(_texParam, _guiTex);
            _renderContext.ModelView = float4x4.Scale(new float3(0.5f, 1, 1))*
                                       float4x4.LookAt(0, 0, 4360, 0, 0, 0, 0, 1, 0);

            _renderContext.Render(_guiPlaneMesh);



            KeyboadInput();
        }

        /// <summary>
        ///     Handles all keyboard input on the GUI.
        /// </summary>
        internal void KeyboadInput()
        {
            // none SysType chosen
            if (Network.Instance.Config.SysType == SysType.None)
            {
                // Choosing
                var lastChosenEntry = _chosenEntry;

                if (Input.Instance.IsKeyDown(KeyCodes.Up) || Input.Instance.GetAxis(InputAxis.MouseWheel) > 0)
                    _chosenEntry = (--_chosenEntry < 0) ? 2 : _chosenEntry;

                if (Input.Instance.IsKeyDown(KeyCodes.Down) || Input.Instance.GetAxis(InputAxis.MouseWheel) < 0)
                    _chosenEntry = (++_chosenEntry > 2) ? 0 : _chosenEntry;

                if (_chosenEntry != lastChosenEntry)
                    _menuSound.Play();

                // Selecting
                if (Input.Instance.IsKeyDown(KeyCodes.Return))
                {
                    _menuSound2.Play();

                    switch (_chosenEntry)
                    {
                        // --> Client
                        case 0:
                            _networkClient = _networkHandler.CreateClient();
                            break;

                        // --> Server
                        case 1:
                            _networkServer = _networkHandler.CreateServer();
                            _networkServer.Startup();
                            break;

                        // --> Exit
                        case 2:
                            Environment.Exit(0);
                            break;
                    }
                }

                return;
            }

            // SysType "Server"
            if (Network.Instance.Config.SysType == SysType.Server)
            {
                if (Input.Instance.IsKeyDown(KeyCodes.Space))
                {
                    _menuSound2.Play();
                    _networkHandler.Mediator.StartGame();
                }

                return;
            }

            // SysType "Client"
            if (Network.Instance.Config.SysType == SysType.Client)
            {
                ConnectToIp = IPInput(ConnectToIp);

                if (Input.Instance.IsKeyDown(KeyCodes.Return))
                {
                    _menuSound2.Play();
                    _networkClient.Startup();
                    _networkClient.ConnectTo(ConnectToIp);
                }
            }
        }

        /// <summary>
        ///     Handles the typing of an IP address on the GUI.
        /// </summary>
        /// <param name="oldIp">The old ip.</param>
        /// <returns>The new ip.</returns>
        internal string IPInput(string oldIp)
        {
            var key = "";

            if (Input.Instance.IsKeyDown(KeyCodes.D0) || Input.Instance.IsKeyDown(KeyCodes.NumPad0))
                key = "0";

            if (Input.Instance.IsKeyDown(KeyCodes.D1) || Input.Instance.IsKeyDown(KeyCodes.NumPad1))
                key = "1";

            if (Input.Instance.IsKeyDown(KeyCodes.D2) || Input.Instance.IsKeyDown(KeyCodes.NumPad2))
                key = "2";

            if (Input.Instance.IsKeyDown(KeyCodes.D3) || Input.Instance.IsKeyDown(KeyCodes.NumPad3))
                key = "3";

            if (Input.Instance.IsKeyDown(KeyCodes.D4) || Input.Instance.IsKeyDown(KeyCodes.NumPad4))
                key = "4";

            if (Input.Instance.IsKeyDown(KeyCodes.D5) || Input.Instance.IsKeyDown(KeyCodes.NumPad5))
                key = "5";

            if (Input.Instance.IsKeyDown(KeyCodes.D6) || Input.Instance.IsKeyDown(KeyCodes.NumPad6))
                key = "6";

            if (Input.Instance.IsKeyDown(KeyCodes.D7) || Input.Instance.IsKeyDown(KeyCodes.NumPad7))
                key = "7";

            if (Input.Instance.IsKeyDown(KeyCodes.D8) || Input.Instance.IsKeyDown(KeyCodes.NumPad8))
                key = "8";

            if (Input.Instance.IsKeyDown(KeyCodes.D9) || Input.Instance.IsKeyDown(KeyCodes.NumPad9))
                key = "9";

            if (Input.Instance.IsKeyDown(KeyCodes.OemPeriod))
                key = ".";

            if (Input.Instance.IsKeyDown(KeyCodes.Back))
                if (oldIp.Length > 0)
                    oldIp = (oldIp == "Discovery?") ? "" : oldIp.Remove(oldIp.Length - 1);

            if (key != "")
                if (oldIp == "Discovery?")
                    oldIp = key;
                else
                    oldIp += key;

            return oldIp;
        }
    }
}