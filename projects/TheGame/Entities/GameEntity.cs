﻿using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    internal class GameEntity
    {
        private readonly int _id;
        private readonly Mediator _mediator;
        protected Mesh EntityMesh;
        private readonly float _collisionRadius;
        private float4x4 _position; //z = Vorne Hinten
        private float4x4 _camMatrix;
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float3 _nRotXV; // normalisierter Richtungsvektor
        private float3 _nRotYV; // normalisierter Richtungsvektor
        private float3 _nRotZV; // normalisierter Richtungsvektor
        private float3 _pointPosition;
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

        internal virtual void OnCollisionEnter(int id)
        {
            Debug.WriteLine("OnCollisionEnter");
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
                    _speed += -2* (float)Time.Instance.DeltaTime * 1.2f;
                }
            }
            else
            {
                if (_speed > 0.2f)
                {
                    _speed = -2 * (float)Time.Instance.DeltaTime / 1.2f;
                }
                else
                {
                    _speed = 0;
                }
            }
        }


        internal float GetSpeed()
        {
            return this._speed;
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
                GameHandler.RemoveBullets.Add(this.GetId());
            }

        }

        internal virtual void Update()
        {
            _nRotXV = float3.Normalize(new float3(_position.M11, _position.M12, _position.M13));
            _nRotYV = float3.Normalize(new float3(_position.M21, _position.M22, _position.M23));
            _nRotZV = float3.Normalize(new float3(_position.M31, _position.M32, _position.M33));

            _position *= float4x4.CreateTranslation(-_position.M41, -_position.M42, -_position.M43)*
                         float4x4.CreateFromAxisAngle(_nRotYV, _rotation.y) * float4x4.CreateFromAxisAngle(_nRotXV, _rotation.x) * //float4x4.CreateRotationX(_rotation.x) *
                         float4x4.CreateTranslation(_position.M41, _position.M42, _position.M43)*
                         float4x4.CreateTranslation(_nRotZV * _speed);
        }

        /// <summary>
        /// Renders the update.
        /// </summary>
        /// <param name="rc">The rc.</param>
        /// <param name="camMatrix">The cam matrix.</param>
        internal virtual void RenderUpdate(RenderContext rc, float4x4 camMatrix)
        {
            _camMatrix = camMatrix;
            //Debug.WriteLine("RenderUpdate");
            //rendern
            rc.SetShader(_sp);
            IShaderParam _vColorParam = _sp.GetShaderParam("vColor");
            _rc.SetShaderParam(_vColorParam, new float4(0.2f,0.5f,0.5f,1));

            //Debug.WriteLine("mtxcam"+(_camMatrix.ToString()));

            _rc.ModelView = _position * _camMatrix;
            //Debug.WriteLine("ModelView" + _rc.ModelView.ToString());
            //Debug.WriteLine("Position" + _position);
            _rc.Render(EntityMesh);
        }

        internal float4x4 GetCamMatrix()
        {
            return float4x4.LookAt(_position.M41 + (_nRotZV.x * 1000), _position.M42 + (_nRotZV.y * 1000), _position.M43 + (_nRotZV.z * 1000), _position.M41,
                                              _position.M42, _position.M43, _position.M21, _position.M22, _position.M23);
        }
    }
}