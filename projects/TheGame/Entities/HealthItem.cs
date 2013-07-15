using Fusee.Math;

namespace Examples.TheGame
{
    internal class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(GameHandler gameHandler, float4x4 position, float speed,
                          float impact, int health)
            : base(gameHandler, position, speed)
        {
            SetId(gameHandler.Mediator.GetObjectId());
            this._collisionRadius = 100;
            _health = health;
            EntityMesh = gameHandler.HealthItemMesh;
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