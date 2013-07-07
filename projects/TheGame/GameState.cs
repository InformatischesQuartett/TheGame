namespace Examples.TheGame
{
    internal class GameState
    {
        internal enum State
        {
            StartMenu,
            InGame,
            GameOver
        };

        private State _curState;
        internal State CurState
        {
            get { return _curState; }
            set { _curState = value; }
        }

        internal GameState(State curState)
        {
            _curState = curState;
        }
    }
}