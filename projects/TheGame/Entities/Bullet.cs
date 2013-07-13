using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Bullet : GameEntity
    {
        private float4x4 _startPos;
        private float _maxDist;
        private float4x4 _endPos;
        private float _distCounter;

        internal Bullet(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed, float impact,
                      float4x4 startPos)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            _startPos = startPos;
            _maxDist = 100;
            //_endPos = this.GetPosition();
            //_endPos.M43 += _maxDist;
            this.EntityMesh = MeshReader.LoadMesh("Assets/Sphere.obj.model");
            Debug.WriteLine("New Bullet");
        }

        internal override void Update()
        {
            base.Update();
            _distCounter += -(this.GetSpeed());
            Debug.WriteLine(_distCounter);
            if (_distCounter > _maxDist)
            {
                this.DestroyEnity();
                Debug.WriteLine("Bullet Destroyed");
            }
        }
        internal override void OnCollisionEnter(int id)
        {
            Debug.WriteLine("Hit Player"+id);
            GameHandler.Players[id].SetLive(-0.5f);
            this.DestroyEnity();
        }
    }
}