using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    public class Player : GameEntity
    {
        private int _life;
        private Time _lastShotTime;


        public Player(int id, Mesh mesh, float collisionRadius, float4x4 position, float speed, float impact)
            : base(id, mesh, collisionRadius, position, speed, impact)
        {
        }

        public int GetLife()
        {
            return _life;
        }
        public void Collision()
        {
            foreach (var go in GameHandler.Players)
            {
                float4x4 distanceMatrix = float4x4.Substract(go.Value.GetPosition(), this.GetPosition());
                var distance = (float)System.Math.Sqrt((System.Math.Pow((double)distanceMatrix.M41, 2) + System.Math.Pow((double)distanceMatrix.M42, 2) + System.Math.Pow((double)distanceMatrix.M43, 2)));
                var distancecoll = go.Value.GetCollisionRadius() + this.GetCollisionRadius();

                if (distance < distancecoll)
                {
                    //BOOOOM
                }
            }
        }
        public void Shoot()
        {

        }
    }

}
