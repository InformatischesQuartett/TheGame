using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Player : GameEntity
    {
        private int _life;
        private Time _lastShotTime;


        internal Player(Mediator mediator, Mesh mesh, float collisionRadius, float4x4 position, float speed,
                      float impact)
            : base(mediator, mesh, collisionRadius, position, speed, impact)
        {
        }

        internal int GetLife()
        {
            return _life;
        }

        internal void Collision()
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
                    //BOOOOM
                }
            }
        }

        internal void Shoot()
        {
            // new Bullet
            // Bullet bullet = new Bullet(NetworkHandler.AssignId(), ....
            var bullet = new Bullet(GetMediator(), null, 4, GetPosition(), 10, 5, GetPosition());
            GameHandler.Items.Add(bullet.GetId(), bullet);
            // add Bullet to ItemDict
        }
    }
}