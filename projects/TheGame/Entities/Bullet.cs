using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Bullet : GameEntity
    {

        private float _maxDist;
        private float4x4 _endPos;
        private float _distCounter;
        private int _ownerId;

        internal Bullet(GameHandler gameHandler, float collisionRadius, float4x4 position, float speed, float impact,
                     int ownerId)
            : base(gameHandler, collisionRadius, position, speed, impact)
        {
            SetId(gameHandler.Mediator.GetObjectId());

            _maxDist = 200;
            _ownerId = ownerId;

            this.EntityMesh = MeshReader.LoadMesh("Assets/Sphere.obj.model");
            Debug.WriteLine("New Bullet");
        }

        internal int GetOwnerId()
        {
            return _ownerId;
        }

        internal override void Update()
        {
            base.Update();
            _distCounter += -0.5f*(this.GetSpeed());
            if (_distCounter > _maxDist)
            {
                this.DestroyEnity();
                Debug.WriteLine("Bullet Destroyed");
            }
        }
        internal override void OnCollisionEnter(int id)
        {
            _gameHandler.Players[id].SetLife(-0.5f);
            _gameHandler.Players[_ownerId].SetScore();
            this.DestroyEnity();
        }
    }
}