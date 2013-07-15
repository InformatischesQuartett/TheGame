using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(GameHandler gameHandler, float4x4 position, float speed)
            : base(gameHandler, position, speed)
        {
            SetId(gameHandler.Mediator.GetObjectId());
            this._collisionRadius = 50;
            _health = 50;
            EntityMesh = gameHandler.HealthItemMesh;

            var explo = new Explosion(GameHandler, GetPosition());

            // Inform other Players
            var data = new DataPacketObjectSpawn
            {
                UserID = GetId(),
                ObjectID = explo.GetId(),
                ObjectType = (int)GameHandler.GameEntities.geHealthItem,
                ObjectVelocity = 0,
                ObjectPosition = explo.GetPositionVector(),
                ObjectRotationX = new float3(0, 0, 0),
                ObjectRotationY = new float3(0, 0, 0),
                ObjectRotationZ = new float3(0, 0, 0)
            };

            var packet = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data };
            GameHandler.Mediator.AddToSendingBuffer(packet, true);
        }

        public int GetHealth()
        {
            return _health;
        }

        internal override void OnCollisionEnter(uint id)
        {
            GameHandler.Players[id].SetLife(+_health);

            DestroyEnity();
        }
        internal override void InstructShader()
        {
            IShaderParam vColorParam = Sp.GetShaderParam("vColor");
            Rc.SetShaderParam(vColorParam, new float4(0.2f, 0.8f, 0.2f, 1));
        }
    }
}