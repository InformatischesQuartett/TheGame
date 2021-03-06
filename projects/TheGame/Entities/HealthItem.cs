﻿using Fusee.Engine;
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
            Rc.SetShaderParam(GameHandler.VColorShaderParam, new float4(0.2f, 0.8f, 0.2f, 1));
        }
    }
}