using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed,
                          float impact, int health)
            : base(mediator, rc, collisionRadius, position, speed, impact)
        {
            _health = health;
        }

        public int GetHealth()
        {
            return _health;
        }

        internal override void OnCollisionEnter(int id)
        {
            GameHandler.Players[id].SetLive(+1);
            this.DestroyEnity();
        }
    }
}