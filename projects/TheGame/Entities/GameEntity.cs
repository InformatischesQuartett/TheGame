using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;



namespace Examples.TheGame.Entities
{
    public class GameEntity
    {
        private int _id;
        private Mesh _mesh;
        private float4x4 _position; //z = Vorne Hinten
        private float2 _rotation; // x = Links Rechts, y = Hoch Runter
        private float _speed;
        private float _impact ;

        public GameEntity (int id, Mesh mesh, float4x4 position, float speed)
        {
            //Attribute initialisieren
            this._id = id;
            this._mesh = mesh;
            this._position = position;
            this._speed = speed;

            //Position des Entitiesan an das Dictionary im GameHandeler geben
            /*
             * gh->insert();
             */
        }

        public void SetRotation(float2 rotation)
        {
            this._rotation += rotation;
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
