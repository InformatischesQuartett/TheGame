using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Networking
{
    class NetworkGUI
    {
        private readonly NetworkHandler _networkHandler;
        private readonly RenderContext _renderContext;

        private NetworkServer _networkServer;
        private NetworkClient _networkClient;

        private readonly Mesh _guiPlaneMesh;
        private readonly ShaderProgram _guiShader;
        private readonly IShaderParam _texParam;

        private ImageData _whiteBg;
        private ITexture _guiTex;
        private string _lastMsg;

        internal string ConnectToIp = "Discovery?";

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkGUI"/> class.
        /// </summary>
        /// <param name="renderContext">Reference to the RenderContext</param>
        /// <param name="networkHandler">Reference to the NetworkHandler</param>
        internal NetworkGUI(RenderContext renderContext, NetworkHandler networkHandler)
        {
            _renderContext = renderContext;
            _networkHandler = networkHandler;

            // load GUIMesh
            _guiPlaneMesh = MeshReader.LoadMesh("Assets/guiPlane.obj.model");

            // load Texture
            _guiShader = MoreShaders.GetShader("texture", _renderContext);
            _texParam = _guiShader.GetShaderParam("texture1");

            RefreshGUITex();
        }

        /// <summary>
        /// Refreshes the GUI.
        /// </summary>
        internal void RefreshGUITex()
        {
            var msg = "The Game\n\n\n\n";

            if (Network.Instance.Config.SysType == SysType.None)
            {
                msg += "F1 = Server \tF2 = Client";
            }
            else
            {
                if (Network.Instance.Config.SysType == SysType.Server)
                    msg += "Server-IP:            " + Network.Instance.LocalIP + "\n\n\n\n\n\n\t            [SPACE]";

                if (Network.Instance.Config.SysType == SysType.Client)
                    if (Network.Instance.Status.Connected)
                        msg += "Verbunden mit:      " + ConnectToIp + "\n\n\n\n\n\n\tWarte auf Spielbeginn";
                    else if (Network.Instance.Status.Connecting)
                        msg += "Verbindungsaufbau...";
                    else
                        msg += "IP eingeben: \t       " + ConnectToIp + "\n\n\t\t\t[RETURN]";
            }

            if (msg != _lastMsg)
            {
                _whiteBg = _renderContext.CreateImage(1920, 1920, "white");
                var finalImg = _renderContext.TextOnImage(_whiteBg, "Calibri", 28, msg, "black", 730, 650);

                _guiTex = _renderContext.CreateTexture(finalImg);
            }

            _lastMsg = msg;
        }

        /// <summary>
        /// Displays the GUI for network settings.
        /// </summary>
        internal void ShowNetworkGUI()
        {
            RefreshGUITex();

            _renderContext.SetShader(_guiShader);
            _renderContext.SetShaderParamTexture(_texParam, _guiTex);
            _renderContext.ModelView = float4x4.LookAt(0, 0, 1000, 0, 0, 0, 0, 1, 0);

            _renderContext.Render(_guiPlaneMesh);

            KeyboadInput();
        }

        /// <summary>
        /// Handles all keyboard input on the GUI.
        /// </summary>
        internal void KeyboadInput()
        {
            // none SysType chosen
            if (Network.Instance.Config.SysType == SysType.None)
            {
                // --> Server
                if (Input.Instance.IsKeyDown(KeyCodes.F1))
                {
                    _networkServer = _networkHandler.CreateServer();
                    _networkServer.Startup();
                }

                // --> Client
                if (Input.Instance.IsKeyDown(KeyCodes.F2))
                {
                    _networkClient = _networkHandler.CreateClient();
                }
            }

            // SysType "Server"
            if (Network.Instance.Config.SysType == SysType.Server)
            {
                if (Input.Instance.IsKeyDown(KeyCodes.Space))
                {
                    // change GameStart -> START
                }
            }

            // SysType "Client"
            if (Network.Instance.Config.SysType == SysType.Client)
            {
                ConnectToIp = IPInput(ConnectToIp);

                if (Input.Instance.IsKeyDown(KeyCodes.Return))
                {
                    _networkClient.Startup();
                    _networkClient.ConnectTo(ConnectToIp);
                }
            }
        }

        /// <summary>
        /// Handles the typing of an IP address on the GUI.
        /// </summary>
        /// <param name="oldIp">The old ip.</param>
        /// <returns>The new ip.</returns>
        public static string IPInput(string oldIp)
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
