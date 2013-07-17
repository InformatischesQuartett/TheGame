using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Player : GameEntity
    {
        private int _life;
        private float _shotTimer;
        private int _score;

        private float2 _mousePos;

        private int _frameCounter;
        private const int FrameUpdate = 10;

        internal Player(GameHandler gameHandler, float4x4 position, float speed, uint id)
            : base(gameHandler, position, speed)
        {
            SetId(id);
            this._collisionRadius = 350;
            EntityMesh = gameHandler.SpaceShipMesh;
            _mousePos = new float2(0, 0);
            Sp = gameHandler.CustomSp;

            _frameCounter = 0;
            
            ResetLife();

            if (System.Math.Abs(_speed) <= _speedMin)
            {
                var sign = System.Math.Sign(_speed);
                _speed = _speedMin * (sign == 0 ? -1 : sign);
            }
        }

        internal int GetLife()
        {
            return _life;
        }

        internal void SetLife(int value)
        {
            if (_life + value > 100)
                ResetLife();
            else
                _life += value;            
        }

        internal void ResetLife()
        {
            _life = 100;
        }

        internal void SetScore()
        {
            _score++;
        }

        internal int GetScore()
        {
            return _score;
        }

        internal override void OnCollisionEnter(uint id)
        {
            SetLife(-1);

            var explo = new Explosion(GameHandler, GetPosition());
            GameHandler.Explosions.Add(explo.GetId(), explo);
        }

        internal void Shoot()
        {
            if (_shotTimer >= 0.25f)
            {
                // new Bullet
                var bullet = new Bullet(GameHandler, GetPosition(), -200, GetId());

                // add Bullet to ItemDict
                GameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
                GameHandler.AudioShoot.Play();

                // Inform other Players
                var data = new DataPacketObjectSpawn
                {
                    UserID = GetId(),
                    ObjectID = bullet.GetId(),
                    ObjectType = (int) GameHandler.GameEntities.geBullet,
                    ObjectVelocity = bullet.GetAbsoluteSpeed(),
                    ObjectPosition = GetPositionVector(),
                    ObjectRotationX = GetRotationFromMatrix(0),
                    ObjectRotationY = GetRotationFromMatrix(1),
                    ObjectRotationZ = GetRotationFromMatrix(2),
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, false);
            }
        }

        internal override void Update()
        {
            base.Update();

            if (GetLife() <= 0)
                DestroyEnity();

            _shotTimer += (float)Time.Instance.DeltaTime;

            // Send update to all clients.
            _frameCounter = ++_frameCounter % FrameUpdate;

            if (_frameCounter == 0)
            {
                var data = new DataPacketPlayerUpdate
                {
                    UserID = GetId(),
                    Timestamp = GameHandler.Mediator.GetUnixTimestamp(),
                    PlayerHealth = _life,
                    PlayerActive = true,
                    PlayerVelocity = GetAbsoluteSpeed(),
                    PlayerPosition = GetPositionVector(),
                    PlayerRotationX = GetRotationFromMatrix(0),
                    PlayerRotationY = GetRotationFromMatrix(1),
                    PlayerRotationZ = GetRotationFromMatrix(2),
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.PlayerUpdate, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, false);
            }
        }

        internal void PlayerInput()
        {
            _mousePos.x = Input.Instance.GetAxis(InputAxis.MouseX);
            _mousePos.y = Input.Instance.GetAxis(InputAxis.MouseY);

            SetRotation(_mousePos);
            
            var speed = 0;

            if (Input.Instance.IsKeyPressed(KeyCodes.W))
                speed = 1;
           if (Input.Instance.IsKeyPressed(KeyCodes.S))
                speed = -1;

            SetSpeed(speed);

            if (Input.Instance.IsButtonDown(MouseButtons.Left) || Input.Instance.IsKeyPressed(KeyCodes.Space))
                Shoot();
        }

        /// <summary>
        /// Instructs the shader prior to rendering
        /// </summary>
        internal override void InstructShader()
        {
            Rc.SetShaderParamTexture(Sp.GetShaderParam("tex"), GameHandler.PlayerTexture);

            Rc.SetShaderParam(Sp.GetShaderParam("calcLighting"), 0);
            Rc.SetShaderParam(Sp.GetShaderParam("ambientLight"), new float4(0.2f, 0.2f, 0.25f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("matAmbient"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("noiseStrength"), 0.0f);
            Rc.SetShaderParam(Sp.GetShaderParam("light1Position"), Rc.View * new float4(1000, 1000, -1000, 1));
            Rc.SetShaderParam(Sp.GetShaderParam("light1Direction"), Rc.InvTransView * new float3(-1, -1, 1));
            Rc.SetShaderParam(Sp.GetShaderParam("light1Diffuse"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("light1Specular"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("matDiffuse"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("matSpecular"), new float4(1.0f, 1.0f, 1.0f, 1.0f));
            Rc.SetShaderParam(Sp.GetShaderParam("matShininess"), 5);
            Rc.SetShaderParam(Sp.GetShaderParam("amountOfLights"), 1);
            //Rc.SetShaderParam(Sp.GetShaderParam("light1Aperture"), 0f);
            Rc.SetShaderParam(Sp.GetShaderParam("light1Falloff"), 0);
        }
    }
}