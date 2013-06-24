using Fusee.Engine;
using Fusee.Math;

namespace Examples.TdM.Networking
{
    class NetworkGUI
    {
        private readonly RenderContext _renderContext;
        private readonly Mesh _guiPlaneMesh;

        private readonly ShaderProgram _guiShader;
        private readonly IShaderParam _texParam;

        private ImageData _whiteBg;
        private ITexture _guiTex;

        internal string ConnectToIp = "Discovery?";

        internal NetworkGUI(RenderContext rc)
        {
            _renderContext = rc;

            // load GUIMesh
            _guiPlaneMesh = MeshReader.LoadMesh("Assets/guiPlane.obj.model");

            // load Texture
            _guiShader = MoreShaders.GetShader("texture", _renderContext);
            _texParam = _guiShader.GetShaderParam("texture1");
            RefreshGUITex(true);
        }

        internal void RefreshGUITex(bool clear = false)
        {
            if (clear)
                _whiteBg = _renderContext.CreateImage(1920, 1920, "white");
            
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

            var finalImg = _renderContext.TextOnImage(_whiteBg, "Calibri", 28, msg, "black", 730, 650);
            _guiTex = _renderContext.CreateTexture(finalImg);
        }

        internal void StartupGUI()
        {
            _renderContext.SetShaderParamTexture(_texParam, _guiTex);
            _renderContext.ModelView = float4x4.LookAt(0, 0, 1000, 0, 0, 0, 0, 1, 0);
            _renderContext.Render(_guiPlaneMesh);

            // none SysType chosen
            if (Network.Instance.Config.SysType == SysType.None)
            {
                // --> Server
                if (Input.Instance.IsKeyDown(KeyCodes.F1))
                {
                    Network.Instance.Config.SysType = SysType.Server;
                    Network.Instance.Config.Discovery = true;
                    Network.Instance.Config.DefaultPort = 14242;

                    Network.Instance.StartPeer();

                    RefreshGUITex(true);
                }

                // --> Client
                if (Input.Instance.IsKeyDown(KeyCodes.F2))
                {
                    Network.Instance.Config.SysType = SysType.Client;
                    Network.Instance.Config.Discovery = true;
                    Network.Instance.Config.ConnectOnDiscovery = true;
                    Network.Instance.Config.DefaultPort = 54954;

                    RefreshGUITex(true);
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
                var newIp = ConnectToIp;
                newIp = NetworkGUIKeys.KeyInput(newIp);

                if (newIp != ConnectToIp)
                {
                    var clear = newIp.Length < ConnectToIp.Length;
                    ConnectToIp = newIp;
                    RefreshGUITex(clear);
                }

                if (Input.Instance.IsKeyDown(KeyCodes.Return))
                {
                    Network.Instance.StartPeer();

                    if (ConnectToIp.Length > 0 && ConnectToIp != "Discovery?")
                        Network.Instance.OpenConnection(ConnectToIp, 14242);
                    else
                        Network.Instance.SendDiscoveryMessage(14242);

                    RefreshGUITex(true);
                }
            }
        }
    }
}
