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
        private int score;



        internal Player(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed,
                      float impact)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            this._life = 5;
            collisionRadius = 10;
            this.EntityMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");
        }

        internal float GetLife()
        {
            return _life;
        }
        internal void SetLive(float value)
        {
            _life += value;
        }

        internal void SetScore()
        {
            this.score++;
        }

        internal int GetScore()
        {
            return score;
        }

        internal void CheckAllCollision()
        {
            foreach (var go in GameHandler.Players)
            {
                if (this.GetId() != go.Value.GetId())
                {
                    if (CheckCollision(go.Value))
                    {
                       // Debug.WriteLine("Collision: Player " + this.GetId() + " BAM with " + go.Value.GetId());
                        // Kill both players
                    }
                    else
                    {
                        //Debug.WriteLine("Collision: Player " + this.GetId() + " clear");
                    }
                }
            }
            foreach (var go in GameHandler.Bullets)
            {
                if (this.GetId() != go.Value.GetOwnerId())
                {
                    if (CheckCollision(go.Value))
                    {
                        Debug.WriteLine("Collision: Bullet " + this.GetId() + " BAM");
                        go.Value.OnCollisionEnter(this.GetId());
                        // Kill bullet
                    }
                    else
                    {
                        //Debug.WriteLine("Collision: Bullet " + go.Value.GetId() + " clear");
                    }
                }
            }
            foreach (var go in GameHandler.HealthItems)
            {
                if (CheckCollision(go.Value))
                {
                    //Debug.WriteLine("Collision: HealthItem " + this.GetId() + " BAM");
                    go.Value.OnCollisionEnter(go.Value.GetId());
                    // Kill healthitem and heal player by impact
                }
                else
                {
                    //Debug.WriteLine("Collision: HealthItem " + this.GetId() + " clear");
                }
            }
        }

        internal override void OnCollisionEnter(int id)
        {
            SetLive(-1);
        }

        internal void Shoot()
        {
            if (_shotTimer >= 0.25f)
            {
                // new Bullet
                var bullet = new Bullet(GetMediator(), this._rc, 4, GetPosition(), -2, 5, this.GetId());
                // add Bullet to ItemDict
                GameHandler.Bullets.Add(bullet.GetId(), bullet);
                _shotTimer = 0;
            }
        }

        internal override void Update()
        {
            base.Update();
            CheckAllCollision();
            _shotTimer += (float)Time.Instance.DeltaTime;

        }

        internal void PlayerInput()
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
            }
            if (Input.Instance.IsKeyPressed(KeyCodes.E))
            {
                Explosion expl = new Explosion(_mediator, _rc, GetPosition());
                GameHandler.Explosions.Add(expl.GetId(), expl);
            this.SetRotation(f);
        }
    }
}