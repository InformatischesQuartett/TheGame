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

        internal Player(GameHandler gameHandler, float4x4 position, float speed,int id)
            : base(gameHandler, position, speed)
        {
            SetId(id);
            this._collisionRadius = 350;
            EntityMesh = gameHandler.SpaceShipMesh;
            _mousePos = new float2(0, 0);
        }

        internal float GetLife()
        {
            return _life;
        }
        internal void SetLife(float value)
        {
            if (_life + value > 100)
            {
                _life = 100;
            }
            else
            {
                _life += value;
            }
            
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

      /*  internal void CheckAllCollision()
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
       */

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
                var bullet = new Bullet(GameHandler, GetPosition(), -150, GetId());

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

            //CheckAllCollision();
            _shotTimer += (float)Time.Instance.DeltaTime;
        }

        internal void PlayerInput()
        {
            _mousePos.x = Input.Instance.GetAxis(InputAxis.MouseX);
            _mousePos.y = Input.Instance.GetAxis(InputAxis.MouseY);

            SetRotation(_mousePos);

            if (Input.Instance.IsKeyPressed(KeyCodes.W))
                SetSpeed(1);

            else if (Input.Instance.IsKeyPressed(KeyCodes.S))
                SetSpeed(-1);

            else
                SetSpeed(0);

            if (Input.Instance.IsButtonDown(MouseButtons.Left) || Input.Instance.IsKeyPressed(KeyCodes.Space))
                Shoot();
        }
    }
}