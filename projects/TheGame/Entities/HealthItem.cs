using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    class HealthItem : GameEntity
    {
        private readonly int _health;

        public HealthItem(NetworkHandler nwHandler, Mesh mesh, float collisionRadius, float4x4 position, float speed, float impact, int health)
            : base(nwHandler, mesh, collisionRadius, position, speed, impact)
        {
            this._health = health;
        }

        public int GetHealth()
        {
            return _health;
        }

    }
}
