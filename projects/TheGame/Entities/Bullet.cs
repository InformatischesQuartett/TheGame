using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class Bullet : GameEntity
    {
        private float4x4 _startPos;
        private float _maxDist;

        internal Bullet(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed, float impact,
                      float4x4 startPos)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            _startPos = startPos;
            _maxDist = 10;
        }
    }
}