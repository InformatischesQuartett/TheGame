using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class Bullet : GameEntity
    {

        private readonly float _maxDist;
        private float _distCounter;
        private readonly uint _ownerId;

        // own bullet
        internal Bullet(GameHandler gameHandler, float4x4 position, float speed, uint ownerId)
            : base(gameHandler, position, speed)
        {
            SetId(gameHandler.Mediator.GetObjectId());

            _maxDist = 5000;
            _ownerId = ownerId;
            this._collisionRadius = 100;
            EntityMesh = gameHandler.BulletMesh;
        }

        // other user's bullet
        internal Bullet(GameHandler gameHandler, float4x4 position, float speed, uint ownerId, uint id)
            : base(gameHandler, position, speed)
        {
            SetId(id);

            _maxDist = 5000;
            _ownerId = ownerId;

            EntityMesh = gameHandler.BulletMesh;
        }

        internal uint GetOwnerId()
        {
            return _ownerId;
        }

        internal override void Update()
        {
            base.Update();
            _distCounter += -0.5f*(GetSpeed());

            if (_distCounter > _maxDist)
            {
                DestroyEnity();
            }
        }
        internal override void OnCollisionEnter(uint id)
        {
            Debug.WriteLine("Kollision");

            GameHandler.Players[id].SetLife(-10);
            var newHealth = GameHandler.Players[id].GetLife();
            Debug.WriteLine("Gesundheit: " + newHealth);
            // Inform specific player!
            if (newHealth <= 0)
                GameHandler.RespawnPlayer(id);
            else
            {
                var data = new DataPacketPlayerUpdate
                {
                    UserID = id,
                    PlayerActive = true,
                    PlayerHealth = newHealth,
                    PlayerVelocity = 0,
                    PlayerPosition = new float3(0, 0, 0),
                    PlayerRotationX = new float3(0, 0, 0),
                    PlayerRotationY = new float3(0, 0, 0),
                    PlayerRotationZ = new float3(0, 0, 0)
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.PlayerUpdate, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, true);
            }

            // GameHandler.Players[_ownerId].SetScore();

            DestroyEnity();
        }
    }
}