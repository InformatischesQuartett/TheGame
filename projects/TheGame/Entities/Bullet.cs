using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    class Bullet : GameEntity
    {
        private float4x4 _startPos;
        private float maxDist;

        public Bullet(int id, Mesh mesh, float4x4 position, float speed, float impact, float4x4 startPos, float4x4 ) : base(id, mesh, position, speed, impact)
        {

        }

        public void Destroy()
        {
            //destroy.this;
        }

    }
}
