using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.TheGame
{
    public class NetworkHandler
    {
        public NetworkHandler()
        {
            //Default Constructor
        }

        public int AssignId()
        {
            int id = 42; //= generate unique id fom Network
            return id; //pass id to a new GameEntity
        }
    }

}
