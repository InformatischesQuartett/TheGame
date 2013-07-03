﻿using System;
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
        private float _maxDist;


        public Bullet(NetworkHandler nwHandler, Mesh mesh, float collisionRadius, float4x4 position, float speed, float impact, float4x4 startPos)
            : base(nwHandler, mesh, collisionRadius, position, speed, impact)
        {
            this._startPos = startPos;
            this._maxDist = 10;
        }
    }
}
