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

        internal Bullet(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed, float impact,
                     int ownerId)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
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
            GameHandler.Players[id].SetLive(-0.5f);
            GameHandler.Players[_ownerId].SetScore();
            this.DestroyEnity();
        }
    }
}