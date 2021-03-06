﻿using System.Diagnostics;
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

            _maxDist = 50;
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

                var data1 = new DataPacketObjectUpdate
                {
                    UserID = GetOwnerId(),
                    ObjectID = GetId(),
                    ObjectType = (int) GameHandler.GameEntities.geBullet,
                    ObjectRemoved = true
                };

                var packet1 = new DataPacket { PacketType = DataPacketTypes.ObjectUpdate, Packet = data1 };
                GameHandler.Mediator.AddToSendingBuffer(packet1, false);
            }
        }
        internal override void OnCollisionEnter(uint id)
        {
            DestroyEnity();
        }
        internal override void InstructShader()
        {
            IShaderParam vColorParam = Sp.GetShaderParam("vColor");
            Rc.SetShaderParam(vColorParam, new float4(1f, 0f, 0f, 1));
        }
    }
}