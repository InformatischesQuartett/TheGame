using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Examples.TheGame
{
    class GameHandlerServer
    {
        private readonly GameHandler _gameHandler;

        private const int PlayAreaRange = 1000;

        private readonly Random _random;

        public GameHandlerServer(GameHandler gameHandler)
        {
            _random = new Random();
            _gameHandler = gameHandler;
        }

        public float RandomNumber()
        {
            const int min = -1 * (PlayAreaRange);
            const int max = PlayAreaRange;

            float rndNum = _random.Next(min, max);
            Debug.WriteLine("RandomNumber" + rndNum);

            return rndNum;
        }

        public float3 RespawnPlayer(int id)
        {
            var rndNum1 = RandomNumber();
            var rndNum2 = RandomNumber();
            var rndNum3 = RandomNumber();

            Debug.WriteLine("RespawnPlayer: " + rndNum1 + ", " + rndNum2 + ", " + rndNum3);
            
            return new float3(rndNum1, rndNum2, rndNum3);
        }
    }
}
