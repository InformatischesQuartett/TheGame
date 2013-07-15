﻿using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Player : GameEntity
    {
        private float _life;
        private float _shotTimer;
        private int _score;

        private float2 _mousePos;

        private int _frameCounter;
        private const int FrameUpdate = 10;

        internal Player(GameHandler gameHandler, float4x4 position, float speed, uint id)
            : base(gameHandler, position, speed)
        {
            SetId(id);
            _life = 5;
            this._collisionRadius = 350;
            EntityMesh = gameHandler.SpaceShipMesh;
            _mousePos = new float2(0, 0);

            _frameCounter = 0;
        }

        internal float GetLife()
        {
            return _life;
        }
        internal void SetLife(float value)
        {
            _life += value;
        }

        internal void ResetLife()
        {
            _life = 5;
        }

        internal void SetScore()
        {
            _score++;
        }

        internal int GetScore()
        {
            return _score;
        }

        internal void CheckAllCollision()
        {
            foreach (var go in GameHandler.Players)
            {
                if (GetId() != go.Value.GetId())
                {
                    if (CheckCollision(go.Value))
                    {
                        Debug.WriteLine("Collision: Player " + GetId() + " BAM with " + go.Value.GetId());
                        // Kill both players
                        DestroyEnity();
                    }
                }
            }

            foreach (var go in GameHandler.Bullets)
            {
                if (GetId() != go.Value.GetOwnerId())
                {
                    if (CheckCollision(go.Value))
                    {
                        go.Value.OnCollisionEnter(GetId());
                    }
                       
                }
            }

            foreach (var go in GameHandler.HealthItems)
            {
                if (CheckCollision(go.Value))
                {
                    go.Value.OnCollisionEnter(go.Value.GetId());
                }
            }
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
                var bullet = new Bullet(GameHandler, GetPosition(), -150, GetId());

                // add Bullet to ItemDict
                GameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
                GameHandler.AudioShoot.Play();

                // Inform other Players
                var data = new DataPacketObjectSpawn
                {
                    UserID = GetId(),
                    ObjectID = (uint) bullet.GetId(),
                    ObjectVelocity = bullet.GetSpeed(),
                    ObjectPosition = GetPositionVector(),
                    ObjectRotationX = GetRotationFromMatrix(0),
                    ObjectRotationY = GetRotationFromMatrix(1),
                    ObjectRotationZ = GetRotationFromMatrix(2),
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, true);
            }
        }

        internal override void Update()
        {
            base.Update();
            if (GetLife() <= 0)
                DestroyEnity();

            CheckAllCollision();
            _shotTimer += (float)Time.Instance.DeltaTime;
        }

        internal void PlayerInput()
        {
            _mousePos.x = Input.Instance.GetAxis(InputAxis.MouseX);
            _mousePos.y = Input.Instance.GetAxis(InputAxis.MouseY);


            if (Input.Instance.IsKeyPressed(KeyCodes.W))
                SetSpeed(1);

            else if (Input.Instance.IsKeyPressed(KeyCodes.S))
                SetSpeed(-1);

            else
                SetSpeed(0);

            if (Input.Instance.IsButtonDown(MouseButtons.Left) || Input.Instance.IsKeyPressed(KeyCodes.Space))
                Shoot();

            SetRotation(_mousePos);

            // Send update to all clients.
            _frameCounter = ++_frameCounter%FrameUpdate;

            if (_frameCounter == 0) {
                var data = new DataPacketPlayerUpdate
                {
                    UserID = GetId(),
                    Timestamp = GameHandler.Mediator.GetUnixTimestamp(),
                    PlayerHealth = (int) _life,
                    PlayerActive = true,
                    PlayerVelocity = GetSpeed(),
                    PlayerPosition = GetPositionVector(),
                    PlayerRotationX = GetRotationFromMatrix(0),
                    PlayerRotationY = GetRotationFromMatrix(1),
                    PlayerRotationZ = GetRotationFromMatrix(2),
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.PlayerUpdate, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, true);
            }
        }
    }
}