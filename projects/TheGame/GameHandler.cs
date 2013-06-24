using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Examples.TdM.Networking;
using Fusee.Engine;
using Fusee.Math;


namespace Examples.TheGame
{
    public class GameHandler
    {
        //RenderContext
        private RenderContext _rc;

        //NetworkManager
        private NetworkHandler _networkHandler;

        //Constructor
        public GameHandler (RenderContext rc) 
        {
            //pass RenderContext
            this._rc = rc;

            //NetworkManager initalize
            _networkHandler = new NetworkHandler(rc);
        }

        //update entity states, is called once per frame
        public void UpdateStates ()
        {
        }

        //is called once per frame to updeate rendering
        public void RenderAFrame ()
        {
            _networkHandler.HandleNetwork();
        }

        //switch games state to an other gamestate
        public void SwitchGameState (/*stateID*/)
        {
            /*Set state id*/
        }
    }
}
