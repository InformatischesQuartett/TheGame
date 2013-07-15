using Fusee.Math;

namespace Examples.TheGame
{
    internal class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(GameHandler gameHandler, float collisionRadius, float4x4 position, float speed,
                          float impact, int health)
            : base(gameHandler, collisionRadius, position, speed, impact)
        {
            SetId(gameHandler.Mediator.GetObjectId());

            _health = health;
        }

        public int GetHealth()
        {
            return _health;
        }

        internal override void OnCollisionEnter(int id)
        {
            GameHandler.Players[id].SetLife(+1);
            DestroyEnity();
        }
    }
}