using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class GameEntity
    {
        private uint _id;

        protected GameHandler GameHandler;

        protected Mesh EntityMesh;
       internal float _collisionRadius;
        private float4x4 _position; //z = Vorne Hinten
        private float4x4 _camMatrix;
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float3 _nRotXV; // normalisierter Richtungsvektor
        private float3 _nRotYV; // normalisierter Richtungsvektor
        private float3 _nRotZV; // normalisierter Richtungsvektor
        private float _scale = 1;
        private float _speed;
        private readonly float _speedMax;
        protected RenderContext Rc;
        protected ShaderProgram Sp;

        internal GameEntity(GameHandler gameHandler, float4x4 position, float speed)
        {
            //Attribute initialisieren
            GameHandler = gameHandler;
            SetPosition(position);
            _speed = speed;
 
            _speedMax = 200;
            Rc = gameHandler.RContext;
            Sp = gameHandler.BasicSp;

            _nRotXV = float3.Normalize(new float3(_position.Row0));
            _nRotYV = float3.Normalize(new float3(_position.Row1));
            _nRotZV = float3.Normalize(new float3(_position.Row2));
        }

        internal uint GetId()
        {
            return _id;
        }

        internal void SetId(uint id)
        {
            _id = id;
        }

        internal float4x4 GetPosition()
        {
            return _position;
        }

        internal void SetPosition(float4x4 position)
        {
            _position = position;
        }

        internal void SetPosition(float3 position)
        {
            _position.Row3 = new float4(position, 1);
        }

        internal float3 GetPositionVector()
        {
            return new float3(_position.M41, _position.M42, _position.M43);
        }

        internal float GetCollisionRadius()
        {
            return _collisionRadius;
        }

        /*internal bool CheckCollision(GameEntity other)
        {
            return (other.GetPositionVector() - GetPositionVector()).LengthSquared <=
                   other.GetCollisionRadius() * other.GetCollisionRadius() +
                   GetCollisionRadius() * GetCollisionRadius();
        }*/

        internal virtual void OnCollisionEnter(uint id)
        {
            Debug.WriteLine("OnCollisionEnter");
        }

        internal void SetRotation(float2 rotation)
        {
            _rotation = rotation;
        }

        internal void SetRotationInMatrix(int axis, float3 value)
        {
            switch (axis)
            {
                case 0:
                    _position.Row0 = new float4(value, 0);
                    break;
                case 1:
                    _position.Row1 = new float4(value, 0);
                    break;
                case 2:
                    _position.Row2 = new float4(value, 0);
                    break;
            }            
        }

        internal float3 GetRotationFromMatrix(int axis)
        {
            switch (axis)
            {
                case 0:
                    return new float3(_position.Row0);
                case 1:
                    return new float3(_position.Row1);
                case 2:
                    return new float3(_position.Row2);
            }
            return new float3(0, 0, 0);
        }



        internal void SetScale(float scale)
        {
            _scale = scale;
        }

        internal float GetScale()
        {
            return _scale;
        }

        internal void SetSpeed(int i)
        {
            // All speeds are negative
            if ((_speed > -_speedMax && i > 0) || (i == 0 && _speed > 0.2f))
            {
                // Vorwärts und bremsen rückwärts
                var newSpeed = _speed + (-100*(float) Time.Instance.DeltaTime*1.2f);

                _speed = i == 0 ? System.Math.Max(+0.2f, newSpeed) : newSpeed;
            }
            else if ((_speed < _speedMax && i < 0) || (i == 0 && _speed < -0.2f))
            {
                // Rückwärts und bremsen vorwärts
                var newSpeed = _speed + (50 * (float)Time.Instance.DeltaTime * 1.2f);
                _speed = i == 0 ? System.Math.Min(-0.2f, newSpeed) : newSpeed;
            }
 
            if (i == 0 && System.Math.Abs(_speed) <= 0.2f)
                _speed = -0.2f*System.Math.Sign(_speed);
        }

        internal int GetSpeed()
        {
            return System.Math.Sign(_speed);
        }

        internal void SetAbsoluteSpeed(float speed)
        {
            _speed = speed;
        }

        internal float GetAbsoluteSpeed()
        {
            return _speed;
        }

        internal void DestroyEnity()
        {
            //Adding Items to RemoveLists
            if (GetType() == typeof (Player))
            {
                var explo = new Explosion(GameHandler, GetPosition());

                // Inform other Players
                var data = new DataPacketObjectSpawn
                {
                    UserID = GetId(),
                    ObjectID = explo.GetId(),
                    ObjectType = 2,
                    ObjectVelocity = 0,
                    ObjectPosition = explo.GetPositionVector(),
                    ObjectRotationX = new float3(0, 0, 0),
                    ObjectRotationY = new float3(0, 0, 0),
                    ObjectRotationZ = new float3(0, 0, 0)
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data };
                GameHandler.Mediator.AddToSendingBuffer(packet, true);

                GameHandler.Explosions.Add(explo.GetId(), explo);
                GameHandler.AudioExplosion.Play();

                GameHandler.RespawnPlayer(GetId());
            }

            if (GetType() == typeof (HealthItem))
            {
                GameHandler.RemoveHealthItems.Add(GetId());
            }

            if (GetType() == typeof(Explosion))
            {
                GameHandler.RemoveExplosions.Add(GetId());
            }

            if (GetType() == typeof (Bullet))
            {
                GameHandler.RemoveBullets.Add(GetId());               
            }
        }

        internal virtual void Update()
        {
            var oldPos3 = new float3(_position.Row3);

            _position *= float4x4.CreateTranslation(-oldPos3) *
                         float4x4.CreateFromAxisAngle(_nRotYV, -_rotation.x) *
                         float4x4.CreateFromAxisAngle(_nRotXV, -_rotation.y) *
                         float4x4.CreateTranslation(oldPos3) *
                         float4x4.CreateTranslation(_nRotZV * _speed);

            _nRotXV = float3.Normalize(new float3(_position.Row0));
            _nRotYV = float3.Normalize(new float3(_position.Row1));
            _nRotZV = float3.Normalize(new float3(_position.Row2));
        }

        /// <summary>
        /// Instructs the shader prior to rendering
        /// </summary>
        internal virtual void InstructShader()
        {
            IShaderParam vColorParam = Sp.GetShaderParam("vColor");
            Rc.SetShaderParam(vColorParam, new float4(0.2f, 0.5f, 0.5f, 1));
        }

        /// <summary>
        /// Renders the update.
        /// </summary>
        /// <param name="rc">The rc.</param>
        /// <param name="camMatrix">The cam matrix.</param>
        internal virtual void RenderUpdate(RenderContext rc, float4x4 camMatrix)
        {
            _camMatrix = camMatrix;

            rc.SetShader(Sp);
            InstructShader();

            Rc.ModelView = float4x4.Scale(_scale) * _position * _camMatrix;

            Rc.Render(EntityMesh);
        }

        internal float4x4 GetCamMatrix()
        {
            return float4x4.LookAt(_position.M41 + (_nRotZV.x * 1000), _position.M42 + (_nRotZV.y * 1000), _position.M43 + (_nRotZV.z * 1000),
                                   _position.M41,                      _position.M42,                      _position.M43,
                                   _position.M21,                      _position.M22,                      _position.M23)
                                   * float4x4.CreateTranslation(0, -300, 0);
        }
    }
}