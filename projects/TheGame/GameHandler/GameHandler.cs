using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Examples.TheGame.Shader;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    internal class GameHandler
    {
        private GameHandlerServer _gameHandlerServer;

        /// <summary>
        ///     Disctionarires mit allen Items und Playern
        /// </summary>
        internal readonly Dictionary<int, HealthItem> HealthItems;

        internal readonly Dictionary<int, Bullet> Bullets;
        internal readonly Dictionary<int, Player> Players;

        internal readonly Dictionary<int, Explosion> Explosions;

        internal readonly List<int> RemoveBullets;
        internal readonly List<int> RemovePlayers;
        internal readonly List<int> RemoveHealthItems;
        internal readonly List<int> RemoveExplosions;

        internal readonly ShaderProgram TextureSp;
        internal readonly ShaderProgram BasicSp;
        internal readonly ShaderProgram CustomSp;

        private readonly IShaderParam _skyBoxShaderParam;

        private readonly ITexture _skyBoxTexture;
        internal readonly ITexture TextureExplosionHandle;

        internal readonly IAudioStream AudioSoundtrack;
        internal readonly IAudioStream AudioExplosion;
        internal readonly IAudioStream AudioShoot;
        internal readonly IAudioStream AudioConnectionEstablished;
        internal readonly IAudioStream AudioInitiated;
        internal readonly IAudioStream AudioMissionComplete;

        internal Mesh SpaceShipMesh;
        internal Mesh BulletMesh;
        internal Mesh ExplosionMesh;
        internal Mesh HealthItemMesh;
        private readonly Mesh _skyBoxMesh;

        /// <summary>
        ///     State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        internal int UserID { get; set; }

        internal readonly Mediator Mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        internal readonly RenderContext RContext;

        private float4x4 _camMatrix;
        private int _playerId;

        internal GameHandler(RenderContext rc, Mediator mediator)
        {
            //pass RenderContext
            RContext = rc;
            Mediator = mediator;

            HealthItems = new Dictionary<int, HealthItem>();
            Bullets = new Dictionary<int, Bullet>();
            Players = new Dictionary<int, Player>();
            Explosions = new Dictionary<int, Explosion>();

            RemoveBullets = new List<int>();
            RemovePlayers = new List<int>();
            RemoveHealthItems = new List<int>();
            RemoveExplosions = new List<int>();

            TextureSp = MoreShaders.GetShader("texture2", rc);
            BasicSp = MoreShaders.GetShader("simple", rc);
            CustomSp = rc.CreateShader(ShaderCode.GetVertexShader(), ShaderCode.GetFragmentShader());

            _skyBoxShaderParam = rc.GetShaderParam(TextureSp, "texture1");

            ImageData texture = rc.LoadImage("Assets/ExplosionTexture.jpg");
            TextureExplosionHandle = rc.CreateTexture(texture);

            texture = rc.LoadImage("Assets/skybox.png");
            _skyBoxTexture = rc.CreateTexture(texture);

            AudioSoundtrack = Audio.Instance.LoadFile("Assets/TheGame Soundtrack.ogg");
            AudioExplosion = Audio.Instance.LoadFile("Assets/Explosion_Edited.wav");
            AudioShoot = Audio.Instance.LoadFile("Assets/Laser_Shoot.wav");
            AudioConnectionEstablished = Audio.Instance.LoadFile("Assets/VoiceActConnectionEstablished.wav");
            AudioInitiated = Audio.Instance.LoadFile("Assets/VoiceActInitiated.wav");
            AudioMissionComplete = Audio.Instance.LoadFile("Assets/VoiceActMissionComplete.wav");

            SpaceShipMesh = MeshReader.LoadMesh("Assets/spaceshuttle.obj.model");
            BulletMesh = MeshReader.LoadMesh("Assets/bullet.obj.model");
            ExplosionMesh = MeshReader.LoadMesh("Assets/Sphere.obj.model");
            HealthItemMesh = MeshReader.LoadMesh("Assets/item.obj.model");
            _skyBoxMesh = MeshReader.LoadMesh("Assets/spacebox.obj.model");

            // Start soundtrack
            AudioSoundtrack.Play(true);

            GameState = new GameState(GameState.State.StartMenu);

            _camMatrix = float4x4.Identity;

        }

        internal void Update()
        {
            // Handle Network
            HandleIncomingMessage();
            // Handle Game Server
            _gameHandlerServer.Update();
            // Handle Game
            foreach (var go in HealthItems)
                go.Value.Update();

            foreach (var go in Bullets)
                go.Value.Update();

            foreach (var go in Explosions)
                go.Value.Update();

            foreach (var go in Players)
            {
                if (go.Key != _playerId)
                    go.Value.Update();
            }

            Players[_playerId].PlayerInput();
            Players[_playerId].Update();
            _camMatrix = Players[_playerId].GetCamMatrix();

            foreach (var removePlayer in RemovePlayers)
                Players.Remove(removePlayer);

            foreach (var removeItem in RemoveHealthItems)
                RemoveHealthItems.Remove(removeItem);

            foreach (var removeBullet in RemoveBullets)
                Bullets.Remove(removeBullet);

            foreach (var removeExplosion in RemoveExplosions)
                Explosions.Remove(removeExplosion);

            RemovePlayers.Clear();
            RemoveHealthItems.Clear();
            RemoveBullets.Clear();
            RemoveExplosions.Clear();
        }

        private void HandleIncomingMessage()
        {
            KeyValuePair<DataPacket, bool> recvPacket;
            while ((recvPacket = Mediator.GetFromReceivingBuffer()).Key.Packet != null)
            {
                switch (recvPacket.Key.PacketType)
                {
                    case DataPacketTypes.PlayerSpawn:
                        var playerSpawnData = (DataPacketPlayerSpawn)recvPacket.Key.Packet;

                        // either a spawning position for this client - or someone needs a new spawning position
                        if (!recvPacket.Value)
                        {
                            Players[_playerId].SetPosition(playerSpawnData.SpawnPosition);
                        }
                        else
                        {
                            // SERVER ACTIVITY!
                            var respawnPosition = _gameHandlerServer.RespawnPlayer(playerSpawnData.UserID);

                            while (Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                            {
                                respawnPosition = _gameHandlerServer.RespawnPlayer(playerSpawnData.UserID);
                            }

                            // send back to user
                            var data = new DataPacketPlayerSpawn()
                            {
                                UserID = playerSpawnData.UserID,
                                Spawn = true,
                                SpawnPosition = respawnPosition
                            };

                            var packet = new DataPacket { PacketType = DataPacketTypes.PlayerSpawn, Packet = data };
                            Mediator.AddToSendingBuffer(packet, true);
                        }

                        break;

                    case DataPacketTypes.PlayerUpdate:
                        // TODO: PlayerUpdate
                        break;

                    case DataPacketTypes.ObjectSpawn:
                        // TODO: ObjectSpawn
                        break;

                    case DataPacketTypes.ObjectUpdate:
                        // TODO: ObjectUpdate
                        break;
                }
            }
        }

        internal void Render()
        {
            // Change ViewPort and aspectRatio (fullsize)
            RContext.Viewport(0, 0, Mediator.Width, Mediator.Height);

            var aspectRatio = Mediator.Width / (float) Mediator.Height;
            RContext.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1000000);

            // Render all Objects
            foreach (var go in HealthItems)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Bullets)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Explosions)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Players)
            {
                if (go.Key != _playerId)
                {
                    go.Value.RenderUpdate(RContext, _camMatrix);
                }
            }

            Players[_playerId].RenderUpdate(RContext, _camMatrix);

            // Render SkyBox
            RContext.SetShader(TextureSp);
            RContext.SetShaderParamTexture(_skyBoxShaderParam, _skyBoxTexture);

            var rotation = _camMatrix;
            rotation.Row3 = new float4(0, 0, 0, 1);
            RContext.ModelView = float4x4.Scale(50, 50, 50) * rotation;

            RContext.Render(_skyBoxMesh);
        }

        internal void StartGame()
        {
            UserID = Mediator.UserID;
            _playerId = Mediator.UserID;

            var p = new Player(this, float4x4.Identity, 0, 0, Mediator.UserID);

            Players.Add(Mediator.UserID, p);

            if (_playerId == 0)
            {
                _gameHandlerServer = new GameHandlerServer(this);
                RespawnPlayer(Mediator.UserID);
            }


            this.AddNewPlayer();
        }

        internal void AddNewPlayer()
        {
            var p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(600, 0, 0), 0, 0, 11);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(300f, 0, 0), 0, 0, 22);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(0, 300f, 0), 0, 0, 33);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(0, 0, -300f), 0, 0, 44);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());
        }

        public void RespawnPlayer(int getId)
        {
            if (UserID == 0)
            {
                var respawnPosition = _gameHandlerServer.RespawnPlayer(getId);

                while (Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                {
                    respawnPosition = _gameHandlerServer.RespawnPlayer(getId);
                }

                Players[getId].SetPosition(respawnPosition);
                Players[getId].ResetLife();
            }
            else
            {
                // TODO: Vom Server anfordern...
            }
        }
    }
}