using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Bullet : GameEntity
    {

        private readonly float _maxDist;
        private float _distCounter;
        private readonly int _ownerId;

        internal Bullet(GameHandler gameHandler, float collisionRadius, float4x4 position, float speed, float impact,
                     int ownerId)
            : base(gameHandler, collisionRadius, position, speed, impact)
        {
            SetId(gameHandler.Mediator.GetObjectId());

            _maxDist = 5000;
            _ownerId = ownerId;

            EntityMesh = gameHandler.BulletMesh;
        }

        internal int GetOwnerId()
        {
            return _ownerId;
        }

        internal override void Update()
        {
            base.Update();
            _distCounter += -0.5f*(GetSpeed());

            if (_distCounter > _maxDist)
            {
                DestroyEnity();
                Debug.WriteLine("Bullet Destroyed");
            }
        }
        internal override void OnCollisionEnter(int id)
        {
            GameHandler.Players[id].SetLife(-0.5f);
            GameHandler.Players[_ownerId].SetScore();
            DestroyEnity();
        }
    }
}