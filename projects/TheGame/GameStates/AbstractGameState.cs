using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.TheGame.GameStates
{
    public abstract class AbstractGameState
    {
        public AbstractGameState()
        {
            //DefaultConstuctor
        }

        //Camera
        public abstract int GetID();
    }
}
