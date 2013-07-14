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

        internal Player(GameHandler gameHandler, float collisionRadius, float4x4 position, float speed,
                      float impact, int id)
            : base(gameHandler, collisionRadius, position, speed, impact)
        {
            SetId(id);
            this._life = 5;
            collisionRadius = 10;
            this.EntityMesh = MeshReader.LoadMesh("Assets/Cube.obj.model");
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
            this.score++;
        }

        internal int GetScore()
        {
            return score;
        }

        internal void CheckAllCollision()
        {
            foreach (var go in _gameHandler.Players)
            {
                if (this.GetId() != go.Value.GetId())
                {
                    if (CheckCollision(go.Value))
                    {
                        Debug.WriteLine("Collision: Player " + this.GetId() + " BAM with " + go.Value.GetId());
                        // Kill both players
                        this.DestroyEnity();
                    }
                    else
                    {
                        //Debug.WriteLine("Collision: Player " + this.GetId() + " clear");
                    }
                }
            }

            foreach (var go in _gameHandler.Bullets)
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

            foreach (var go in _gameHandler.HealthItems)
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
            SetLife(-1);

            Explosion explo = new Explosion(_gameHandler, GetPosition());
            _gameHandler.Explosions.Add(explo.GetId(), explo);
        }

        internal void Shoot()
        {
            if (_shotTimer >= 0.25f)
            {
                // new Bullet
                var bullet = new Bullet(_gameHandler, 4, GetPosition(), -2, 5, GetId());

                // add Bullet to ItemDict
                _gameHandler.Bullets.Add(bullet.GetId(), bullet);
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
            /*if (Input.Instance.IsKeyPressed(KeyCodes.E))
            {
                Explosion explo = new Explosion(_gameHandler, GetPosition());
                _gameHandler.Explosions.Add(explo.GetId(), explo);
            }*/
            this.SetRotation(f);
        }
    }
}