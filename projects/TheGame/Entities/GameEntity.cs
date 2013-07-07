using Examples.TheGame.Networking;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    internal class GameEntity
    {
        private readonly int _id;
        private readonly Mediator _mediator;
        private readonly Mesh _mesh;
        private readonly float _collisionRadius;
        private float4x4 _position; //z = Vorne Hinten
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float _speed;
        private float _speedMax;
        private float _impact;

        internal GameEntity(Mediator mediator, Mesh mesh, float collisionRadius, float4x4 position, float speed,
                            float impact)
        {
            //Attribute initialisieren
            _mediator = mediator;
            _id = _mediator.GetObjectId();
            _mesh = mesh;
            _collisionRadius = collisionRadius;
            _position = position;
            _speed = speed;
            _impact = impact;
            _speedMax = 10;

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
            _rotation += rotation * (float)Time.Instance.DeltaTime;
        }

        internal void SetSpeed(int speed)
        {
            if (_speed < _speedMax)
            {
                _speed += speed * (float)Time.Instance.DeltaTime;
            }
            else
            {
                _speed = _speedMax;
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
            _position *= float4x4.CreateRotationY(_rotation.y)*float4x4.CreateRotationX(_rotation.x)*
                         float4x4.CreateTranslation(0, 0, _speed);
        }

        internal virtual void RenderUpdate(RenderContext rc)
        {
            //rendern
            rc.ModelView = _position;
            rc.Render(_mesh);
        }
    }
}