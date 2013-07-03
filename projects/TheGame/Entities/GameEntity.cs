using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame.Entities
{
    public class GameEntity
    {
        private int _id;
        private NetworkHandler _nwHandler;
        private readonly Mesh _mesh;
        private readonly float _collisionRadius;
        private float4x4 _position; //z = Vorne Hinten
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float _speed;
        private float _impact ;

        public GameEntity(NetworkHandler nwHandler, Mesh mesh, float collisionRadius, float4x4 position, float speed, float impact)
        {
            //Attribute initialisieren
            this._nwHandler = nwHandler;
            this._id = _nwHandler.AssignId();
            this._mesh = mesh;
            this._collisionRadius = collisionRadius;
            this._position = position;
            this._speed = speed;
            this._impact = impact;

            //Position des Entitiesan an das Dictionary im GameHandeler geben
            /*
             * gh->insert();
             */
        }


        public NetworkHandler GetNWHandler()
        {
            return _nwHandler;
        }

        public int GetId()
        {
            return _id;
        }

        public float4x4 GetPosition()
        {
            return _position;
        }

        public float GetCollisionRadius()
        {
            return _collisionRadius;
        }

        public void SetRotation(float2 rotation)
        {
            this._rotation += rotation;
        }

        public void DestroyEnity()
        {
            //remove this from Dictionary
        }

        public virtual void Update ()
        {
            _position *= float4x4.CreateRotationY(_rotation.y) * float4x4.CreateRotationX(_rotation.x) * float4x4.CreateTranslation(0,0,_speed);
        }

        public virtual void RenderUpdate(RenderContext rc)
        {
            //rendern
        }
    }
}
