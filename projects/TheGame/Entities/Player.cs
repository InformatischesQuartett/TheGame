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
            foreach (var go in _gameHandler.Players)
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

            foreach (var go in _gameHandler.Bullets)
            {
                if (GetId() != go.Value.GetOwnerId())
                {
                    if (CheckCollision(go.Value))
                    {
                        Debug.WriteLine("Collision: Bullet " + GetId() + " BAM");
                        go.Value.OnCollisionEnter(GetId());
                        // Kill bullet
                    }
                }
            }

            foreach (var go in _gameHandler.HealthItems)
            {
                if (CheckCollision(go.Value))
                {
                    //Debug.WriteLine("Collision: HealthItem " + this.GetId() + " BAM");
                    go.Value.OnCollisionEnter(go.Value.GetId());
                    // Kill healthitem and heal player by impact
                }
            }
        }

        internal override void OnCollisionEnter(int id)
        {
            SetLife(-1);

            var explo = new Explosion(_gameHandler, GetPosition());
            _gameHandler.Explosions.Add(explo.GetId(), explo);
        }

        internal void Shoot()
        {
            if (_shotTimer >= 0.25f)
            {
                // new Bullet
                var bullet = new Bullet(_gameHandler, 4, GetPosition(), -150, 5, GetId());

                // add Bullet to ItemDict
                _gameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
                _gameHandler.AudioShoot.Play();
            }
        }

        internal override void Update()
        {
            base.Update();
            if (GetLife() <= 0)
            {
                DestroyEnity();
            }

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
            {
                SetSpeed(1);
            }
            else if (Input.Instance.IsKeyPressed(KeyCodes.S))
            {
                SetSpeed(-1);
            }
            else
            {
                SetSpeed(0);
            }
            //Shoot on left mouse button
            if (Input.Instance.OnButtonDown(MouseButtons.Left))
            {
                Shoot();
            }
            /*if (Input.Instance.IsKeyPressed(KeyCodes.E))
            {
                Explosion explo = new Explosion(_gameHandler, GetPosition());
                _gameHandler.Explosions.Add(explo.GetId(), explo);
            }*/
            SetRotation(_mousePos);
        }
    }
}