using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class Bullet : GameEntity
    {
        private float4x4 _startPos;
        private float _maxDist;
        private float4x4 _endPos; 

        internal Bullet(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed, float impact,
                      float4x4 startPos)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            _startPos = startPos;
            _maxDist = 10;
            _endPos = this.GetPosition();
            _endPos.M43 += _maxDist;
        }

        internal override void Update()
        {
            base.Update();

            if (this.GetPosition() == _endPos)
            {
                this.DestroyEnity();
            }
        }
        internal override void OnCollisionEnter(int id)
        {
            GameHandler.Players[id].SetLive(-0.5f);
            this.DestroyEnity();
        }
    }
}