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
        internal readonly Dictionary<uint, HealthItem> HealthItems;

        internal readonly Dictionary<uint, Bullet> Bullets;
        internal readonly Dictionary<uint, Player> Players;

        internal readonly Dictionary<uint, Explosion> Explosions;

        internal enum GameEntities
        {
            geBullet=0,
            geHealthItem=1,
            geExplosion=2
        }

        internal readonly List<uint> RemoveBullets;
        internal readonly List<uint> RemovePlayers;
        internal readonly List<uint> RemoveHealthItems;
        internal readonly List<uint> RemoveExplosions;

        internal readonly ShaderProgram TextureSp;
        internal readonly ShaderProgram BasicSp;
        internal readonly ShaderProgram CustomSp;

        private readonly IShaderParam _skyBoxShaderParam;
        internal readonly IShaderParam PlayerShaderParam;

        private readonly ITexture _skyBoxTexture;
        internal readonly ITexture TextureExplosionHandle;
        internal readonly ITexture PlayerTexture;

        private readonly ITexture _healthBarGreenTexture;
        private readonly ITexture _healthBarOrangeTexture;
        private readonly ITexture _healthBarRedTexture;
        private ITexture _currentHealthBarTexture;

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
        private readonly Mesh _guiCube;

        /// <summary>
        ///     State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        internal uint UserID { get; set; }

        internal readonly Mediator Mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        internal readonly RenderContext RContext;

        private float4x4 _camMatrix;


        internal GameHandler(RenderContext rc, Mediator mediator)
        {
            //pass RenderContext
            RContext = rc;
            Mediator = mediator;

            HealthItems = new Dictionary<uint, HealthItem>();
            Bullets = new Dictionary<uint, Bullet>();
            Players = new Dictionary<uint, Player>();
            Explosions = new Dictionary<uint, Explosion>();

            RemoveBullets = new List<uint>();
            RemovePlayers = new List<uint>();
            RemoveHealthItems = new List<uint>();
            RemoveExplosions = new List<uint>();

            TextureSp = MoreShaders.GetShader("texture2", rc);
            BasicSp = MoreShaders.GetShader("simple", rc);
            CustomSp = rc.CreateShader(ShaderCode.GetVertexShader(), ShaderCode.GetFragmentShader());

            _skyBoxShaderParam = rc.GetShaderParam(TextureSp, "texture1");
            PlayerShaderParam = rc.GetShaderParam(TextureSp, "texture1");

            ImageData texture = rc.LoadImage("Assets/ExplosionTexture.jpg");
            TextureExplosionHandle = rc.CreateTexture(texture);

            texture = rc.LoadImage("Assets/skybox.png");
            _skyBoxTexture = rc.CreateTexture(texture);
            texture = rc.LoadImage("Assets/playertex.jpg");
            PlayerTexture = rc.CreateTexture(texture);

            texture = rc.LoadImage("Assets/Topbar_green.png");
            _healthBarGreenTexture = rc.CreateTexture(texture);
            texture = rc.LoadImage("Assets/Topbar_orange.png");
            _healthBarOrangeTexture = rc.CreateTexture(texture);
            texture = rc.LoadImage("Assets/Topbar_red.png");
            _healthBarRedTexture = rc.CreateTexture(texture);

            _currentHealthBarTexture = _healthBarGreenTexture;


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
            _guiCube = MeshReader.LoadMesh("Assets/Cube.obj.model");

            // Start soundtrack
            AudioSoundtrack.Play(true);

            GameState = new GameState(GameState.State.StartMenu);

            _camMatrix = float4x4.Identity;

        }

        internal void Update()
        {
            // Handle Network
            HandleIncomingMessage();

            if (UserID == 0)
            {
                // Handle Game Server
                _gameHandlerServer.Update();
            }

            // Handle Game
            foreach (var go in HealthItems)
                go.Value.Update();

            foreach (var go in Bullets)
                go.Value.Update();

            foreach (var go in Explosions)
                go.Value.Update();

            foreach (var go in Players)
            {
                if (go.Key != UserID)
                    go.Value.Update();
            }

            Players[UserID].PlayerInput();
            Players[UserID].Update();
            _camMatrix = Players[UserID].GetCamMatrix();

            foreach (var removePlayer in RemovePlayers)
                Players.Remove(removePlayer);

            foreach (var removeItem in RemoveHealthItems)
            {
                HealthItems.Remove(removeItem);
                _gameHandlerServer.SpawnHealthItem();
            }
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
                        var playerSpawnData = (DataPacketPlayerSpawn) recvPacket.Key.Packet;

                        // either a spawning position for this client - or someone needs a new spawning position
                        if (!recvPacket.Value)
                        {
                            Players[UserID].SetPosition(playerSpawnData.SpawnPosition);
                            Players[UserID].ResetLife();
                        }
                        else
                        {
                            // SERVER ACTIVITY!
                            RespawnPlayer(playerSpawnData.UserID);
                        }

                        break;

                    case DataPacketTypes.PlayerUpdate:
                        var playerUpdateData = (DataPacketPlayerUpdate) recvPacket.Key.Packet;

                        var userID = playerUpdateData.UserID;

                        // This player got hit!
                        if (userID == UserID)
                        {
                            Debug.WriteLine("Setze Gesundheit auf: " + playerUpdateData.PlayerHealth);
                            Players[userID].SetLife(playerUpdateData.PlayerHealth);
                            break;
                        }

                        if (!Players.ContainsKey(userID))
                        {
                            var p = new Player(this, float4x4.Identity, 0, userID);
                            Players.Add(userID, p);
                        }

                        Players[userID].SetPosition(playerUpdateData.PlayerPosition);

                        Players[userID].SetRotationInMatrix(0, playerUpdateData.PlayerRotationX);
                        Players[userID].SetRotationInMatrix(1, playerUpdateData.PlayerRotationY);
                        Players[userID].SetRotationInMatrix(2, playerUpdateData.PlayerRotationZ);

                        Players[userID].SetAbsoluteSpeed(playerUpdateData.PlayerVelocity);

                        break;

                    case DataPacketTypes.ObjectSpawn:
                        var objectSpawnData = (DataPacketObjectSpawn) recvPacket.Key.Packet;

                        var objectID = objectSpawnData.ObjectID;
                        var ownerID = objectSpawnData.UserID;

                        switch ((GameEntities) objectSpawnData.ObjectType)
                        {
                            case GameEntities.geBullet:
                                if (!Bullets.ContainsKey(objectID))
                                {
                                    var b = new Bullet(this, float4x4.Identity, 0, ownerID, objectID);
                                    Bullets.Add(objectID, b);
                                }

                                Bullets[objectID].SetPosition(objectSpawnData.ObjectPosition);

                                Bullets[objectID].SetRotationInMatrix(0, objectSpawnData.ObjectRotationX);
                                Bullets[objectID].SetRotationInMatrix(1, objectSpawnData.ObjectRotationY);
                                Bullets[objectID].SetRotationInMatrix(2, objectSpawnData.ObjectRotationZ);

                                Bullets[objectID].SetAbsoluteSpeed(objectSpawnData.ObjectVelocity);

                                break;

                            case GameEntities.geExplosion:
                                if (!Explosions.ContainsKey(objectID))
                                {
                                    var b = new Explosion(this, float4x4.Identity, objectID);
                                    Explosions.Add(objectID, b);
                                }

                                Explosions[objectID].SetPosition(objectSpawnData.ObjectPosition);
                                AudioExplosion.Play();

                                break;
                        }

                        break;

                    case DataPacketTypes.ObjectUpdate:
                        var objectUpdateData = (DataPacketObjectUpdate) recvPacket.Key.Packet;

                        var objectUpdateID = objectUpdateData.ObjectID;

                        switch (objectUpdateData.ObjectType)
                        {
                            case 0:
                                if (Bullets.ContainsKey(objectUpdateID))
                                {
                                    if (objectUpdateData.ObjectRemoved)
                                        RemoveBullets.Add(objectUpdateData.ObjectID);
                                }

                                break;
                        }

                        break;
                }
            }
        }

        internal void Render()
        {
            // Change ViewPort and aspectRatio (fullsize)
            RContext.Viewport(0, 0, Mediator.Width, Mediator.Height);

            var aspectRatio = Mediator.Width/(float) Mediator.Height;
            RContext.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1000000);

            // Render SkyBox
            RContext.SetShader(TextureSp);
            RContext.SetShaderParamTexture(_skyBoxShaderParam, _skyBoxTexture);

            var rotation = _camMatrix;
            rotation.Row3 = new float4(0, 0, 0, 1);
            RContext.ModelView = float4x4.Scale(50, 50, 50) * rotation;

            RContext.Render(_skyBoxMesh);

            RContext.Clear(ClearFlags.Depth);

            // Render all Objects
            foreach (var go in HealthItems)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Bullets)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Explosions)
                go.Value.RenderUpdate(RContext, _camMatrix);

            foreach (var go in Players)
            {
                if (go.Key != UserID)
                {
                    go.Value.RenderUpdate(RContext, _camMatrix);
                }
            }

            Players[UserID].RenderUpdate(RContext, _camMatrix);
           
            //Render Gui Cube for Healthbar
            RContext.Clear(ClearFlags.Depth);

            if (Players[UserID].GetLife() >= 70)
                _currentHealthBarTexture = _healthBarGreenTexture;

            if (Players[UserID].GetLife() < 70)
                _currentHealthBarTexture = _healthBarOrangeTexture;

            if (Players[UserID].GetLife() <= 40)
                _currentHealthBarTexture = _healthBarRedTexture;

            RContext.SetShader(TextureSp);
            RContext.SetShaderParamTexture(_skyBoxShaderParam, _currentHealthBarTexture);
            RContext.ModelView = float4x4.Scale(1, 0.075f, 0.001f) * float4x4.CreateTranslation(0, 75, -200);
            RContext.Render(_guiCube);
        }

        internal void StartGame()
        {
            UserID = Mediator.UserID;

            var p = new Player(this, float4x4.Identity, 0, Mediator.UserID);
            Players.Add(Mediator.UserID, p);

            if (UserID == 0)
            {
                _gameHandlerServer = new GameHandlerServer(this);
                RespawnPlayer(Mediator.UserID);
            }

            //this.AddNewPlayer();
        }

        internal void AddNewPlayer()
        {
            var p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(600, 0, 0), 0, 11);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(300f, 0, 0), 0, 22);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(0, 300f, 0), 0, 33);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());

            p = new Player(this, float4x4.Identity*float4x4.CreateTranslation(0, 0, -300f), 0, 44);
            Players.Add(p.GetId(), p);
            RespawnPlayer(p.GetId());
        }

        public void RespawnPlayer(uint getId)
        {
            if (getId == 0 && UserID == 0)
            {
                var respawnPosition = _gameHandlerServer.RandomPosition();

                while (Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                {
                    respawnPosition = _gameHandlerServer.RandomPosition();
                }

                Players[getId].SetPosition(respawnPosition);
                Players[getId].ResetLife();
            }
            else
            {
                // SERVER ACTIVITY!
                var respawnPosition = _gameHandlerServer.RandomPosition();

                while (Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                {
                    respawnPosition = _gameHandlerServer.RandomPosition();
                }

                // send back to user
                var data = new DataPacketPlayerSpawn
                {
                    UserID = getId,
                    Spawn = true,
                    SpawnPosition = respawnPosition
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.PlayerSpawn, Packet = data };
                Mediator.AddToSendingBuffer(packet, true);

                // reset life
                if (!Players.ContainsKey(getId))
                {
                    var p = new Player(this, float4x4.Identity, 0, getId);
                    Players.Add(getId, p);
                }

                Players[getId].SetPosition(respawnPosition);
                Players[getId].ResetLife();
            }
        }
    }
}