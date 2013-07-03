using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.TheGame
{
    public class GameState
    {
        public enum State
        {
            StartMenu,
            InGame,
            GameOver
        };

        private State _curState;

        public GameState (State curState)
        {
            _curState = curState;
        }

        public State GetSate ()
        {
            return _curState;
        }

        public void SetState(State newState)
        {
            _curState = newState;
        }
      
    }
}
