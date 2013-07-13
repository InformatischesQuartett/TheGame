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

        private const int PlayAreaRange = 1500;

        public GameHandlerServer(GameHandler gameHandler)
        {
            _gameHandler = gameHandler;
        }

        public float RandomNumber()
        {
            var random = new Random();

            const int min = -1 * (PlayAreaRange);
            const int max = PlayAreaRange;

            float rndNum = random.Next(min, max);
            Debug.WriteLine("RandomNumber" + rndNum);

            return rndNum;
        }

        public float3 RespawnPlayer(int id)
        {
            Debug.WriteLine("RespawnPlayer:" + id);

            var rndNum = RandomNumber()*id;
            return new float3(rndNum, rndNum, rndNum);
        }
    }
}
