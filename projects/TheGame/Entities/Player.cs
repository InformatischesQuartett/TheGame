using System;
using System.Diagnostics;
using Examples.TheGame.Networking;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class Player : GameEntity
    {
        private int _life;
        private float _shotTimer;



        internal Player(Mediator mediator, Mesh mesh, float collisionRadius, float4x4 position, float speed,
                      float impact)
            : base(mediator, mesh, collisionRadius, position, speed, impact)
        {
            this._life = 3;
            collisionRadius = 10;
        }

        internal int GetLife()
        {
            return _life;
        }

        internal void CheckCollision()
        {
            foreach (var go in GameHandler.Players)
            {
                var distanceMatrix = float4x4.Substract(go.Value.GetPosition(), GetPosition());
                var distance =
                    (float)
                    Math.Sqrt((Math.Pow(distanceMatrix.M41, 2) + Math.Pow(distanceMatrix.M42, 2) +
                               Math.Pow(distanceMatrix.M43, 2)));
                var distancecoll = go.Value.GetCollisionRadius() + GetCollisionRadius();

                if (distance < distancecoll)
                {
                    if (go.GetType() == typeof (Player))
                        this.DestroyEnity();
                    
                    go.Value.DestroyEnity();
                }
            }
        }

        internal void Shoot()
        {
            if (_shotTimer >= 0.25f)
            {
                // new Bullet
                var bullet = new Bullet(GetMediator(), null, 4, GetPosition(), 10, 5, GetPosition());
                // add Bullet to ItemDict
                GameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
            }
        }

        internal override void Update()
        {
            base.Update();
            PlayerInput();
            CheckCollision();
            _shotTimer += (float)Time.Instance.DeltaTime;

        }

        private void PlayerInput()
        {

           // move forward Shift
            this.SetSpeed(Input.Instance.IsKeyDown(KeyCodes.Shift) ? 1 : 0);
            //Up  Down
            if (Input.Instance.IsKeyDown(KeyCodes.W))
            {
                var f = new float2(1,0);
                this.SetRotation(f);
            }
            if (Input.Instance.IsKeyDown(KeyCodes.W))
            {
                var f = new float2(-1,0);
                this.SetRotation(f);
            }
            //Left Right
            if (Input.Instance.IsKeyDown(KeyCodes.A))
            {
                var f = new float2(0,-1);
                this.SetRotation(f);
            }
            if (Input.Instance.IsKeyDown(KeyCodes.D))
            {
                var f = new float2(0, 1);
                this.SetRotation(f);
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Space))
            {
                this.Shoot();
            }
        }
    }
}