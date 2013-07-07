using System.Diagnostics;
using Examples.TheGame.Networking;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class GameEntity
    {
        private readonly int _id;
        private readonly Mediator _mediator;
        protected Mesh EntityMesh;
        private readonly float _collisionRadius;
        private float4x4 _position; //z = Vorne Hinten
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float3 _nRotV; // normalisierter Richtivsvektor
        private float _speed;
        private float _speedMax;
        private float _impact;
        protected RenderContext _rc;
        private ShaderProgram _sp;

        internal GameEntity(Mediator mediator, RenderContext rc, float collisionRadius, float4x4 position, float speed,
                            float impact)
        {
            //Attribute initialisieren
            _mediator = mediator;
            _id = _mediator.GetObjectId();
            _collisionRadius = collisionRadius;
            _position = position;
            _speed = speed;
            _impact = impact;
            _speedMax = 10;
            _rc = rc;
            _sp = MoreShaders.GetShader("simple", _rc);
        }


        internal Mediator GetMediator()
        {
            return _mediator;
        }

        internal int GetId()
        {
            return _id;
        }

        internal float4x4 GetPosition()
        {
            return _position;
        }

        internal float GetCollisionRadius()
        {
            return _collisionRadius;
        }

        internal void SetRotation(float2 rotation)
        {
            _rotation = rotation * (float)Time.Instance.DeltaTime;
        }

        internal void SetSpeed(bool power)
        {
            if(power == true)
            {
                if (_speed < _speedMax)
                {
                    _speed += -1* (float)Time.Instance.DeltaTime * 1.2f;
                }
            }
            else
            {
                if (_speed > 0.2f)
                {
                    _speed = -1 * (float)Time.Instance.DeltaTime / 1.2f;
                }
                else
                {
                    _speed = 0;
                }
            }
        }


        internal void DestroyEnity()
        {
            if (this.GetType() == typeof (Player) || this.GetType() == typeof (HealthItem))
            {
                //Respawn stuff
            }
            if (this.GetType() == typeof (Bullet))
            {
                //remove Bullet from Dict
            }

        }

        internal virtual void Update()
        {
            _nRotV = float3.Normalize(new float3(_position.M31, _position.M32, _position.M33));

            _position *= float4x4.CreateTranslation(-_position.M41, -_position.M42, -_position.M43)*
                         float4x4.CreateRotationY(_rotation.y)*float4x4.CreateRotationX(_rotation.x)*
                         float4x4.CreateTranslation(_position.M41, _position.M42, _position.M43)*
                         float4x4.CreateTranslation(_nRotV * _speed);
        }

        internal virtual void RenderUpdate(RenderContext rc)
        {
            Debug.WriteLine("RenderUpdate");
            //rendern
            rc.SetShader(_sp);
            IShaderParam _vColorParam = _sp.GetShaderParam("vColor");
            _rc.SetShaderParam(_vColorParam, new float4(0.2f,0.5f,0.5f,1));

            float4x4 mtxCam = float4x4.LookAt(_position.M41 + (_nRotV.x * 1000), _position.M42 + (_nRotV.y * 1000), _position.M43 + (_nRotV.z * 1000), _position.M41,
                                              _position.M42, _position.M43, _position.M21, _position.M22, _position.M23);
            Debug.WriteLine("mtxcam"+(mtxCam.ToString()));

            _rc.ModelView = _position *mtxCam;
            Debug.WriteLine("ModelView" + _rc.ModelView.ToString());
            Debug.WriteLine("Position" + _position);
            _rc.Render(EntityMesh);
        }
    }
}