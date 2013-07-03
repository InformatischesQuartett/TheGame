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

        internal GameState(State curState)
        {
            _curState = curState;
        }

        private State GetSate()
        {
            return _curState;
        }

        private void SetState(State newState)
        {
            _curState = newState;
        }
    }
}