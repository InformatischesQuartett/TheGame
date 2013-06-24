using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fusee.Engine;
using Fusee.Math;


namespace Examples.TheGame
{
    public class GameHandler
    {
        //RenderContext
        private RenderContext _rc;

        //NetworkManager


        //Constructor
        public GameHandler (RenderContext rc) 
        {
            //pass RenderContext
            this._rc = rc;

            //NetworkManager initalize
        }

        //update entity states, is called once per frame
        public void UpdateStates ()
        {
        }

        //is called once per frame to updeate rendering
        public void RenderAFrame ()
        {
        }

        //switch games state to an other gamestate
        public void SwitchGameState (/*stateID*/)
        {
            /*Set state id*/
        }
    }
}
