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



        internal Player(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed,
                      float impact)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            this._life = 3;
            collisionRadius = 10;
            this.EntityMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");

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
                var bullet = new Bullet(GetMediator(), this._rc, 4, GetPosition(), 10, 5, GetPosition());
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

            var f = new float2(0, 0);
            
           // move forward Shift
            switch (Input.Instance.IsKeyPressed(KeyCodes.P))
            {
                case true:
                    SetSpeed(true);
                    break;
                case false:
                    SetSpeed(false);
                    break;
                default:
                    break;
            }


            //Up  Down
            if (Input.Instance.IsKeyPressed(KeyCodes.W))
            {
                f += new float2(-1,0);
            }
            if (Input.Instance.IsKeyPressed(KeyCodes.S))
            {
                f += new float2(1,0);
            }
            if (Input.Instance.IsKeyPressed(KeyCodes.A))
            {
                f += new float2(0,1);
            }

            if (Input.Instance.IsKeyPressed(KeyCodes.D))
            {
                f += new float2(0, -1);
            }
            if (Input.Instance.IsKeyPressed(KeyCodes.Space))
            {
                this.Shoot();
            }
            this.SetRotation(f);
        }
    }
}