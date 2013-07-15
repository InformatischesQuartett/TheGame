using System;
using System.Diagnostics;
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

        internal Player(GameHandler gameHandler, float collisionRadius, float4x4 position, float speed,
                      float impact, int id)
            : base(gameHandler, collisionRadius, position, speed, impact)
        {
            SetId(id);
            _life = 5;
            collisionRadius = 10;
            EntityMesh = gameHandler.SpaceShipMesh;
            _mousePos = new float2(0, 0);
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

        internal override void OnCollisionEnter(int id)
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
                var bullet = new Bullet(GameHandler, 4, GetPosition(), -150, 5, GetId());

                // add Bullet to ItemDict
                GameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
                GameHandler.AudioShoot.Play();
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
            var xDiff = Input.Instance.GetAxis(InputAxis.MouseX) / 100;
            var yDiff = Input.Instance.GetAxis(InputAxis.MouseY) / 100;

            _mousePos.x *= (float)Math.Exp(-0.98 * Time.Instance.DeltaTime); 
            _mousePos.y *= (float)Math.Exp(-0.98 * Time.Instance.DeltaTime); 

            if (Math.Abs(xDiff) > MathHelper.EpsilonFloat)
                _mousePos.x = xDiff;

            if (Math.Abs(yDiff) > MathHelper.EpsilonFloat)
                _mousePos.y = yDiff;

            if (Input.Instance.IsKeyPressed(KeyCodes.W))
                SetSpeed(1);

            else if (Input.Instance.IsKeyPressed(KeyCodes.S))
                SetSpeed(-1);

            else
                SetSpeed(0);

            if (Input.Instance.OnButtonDown(MouseButtons.Left))
                Shoot();

            SetRotation(_mousePos);
        }
    }
}