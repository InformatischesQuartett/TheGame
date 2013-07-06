using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(Mediator.Mediator mediator, Mesh mesh, float collisionRadius, float4x4 position, float speed,
                          float impact, int health)
            : base(mediator, mesh, collisionRadius, position, speed, impact)
        {
            _health = health;
        }

        public int GetHealth()
        {
            return _health;
        }
    }
}