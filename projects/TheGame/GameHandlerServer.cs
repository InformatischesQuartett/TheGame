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
        public GameHandlerServer()
        {
            
        }

        public void RespawnPlayer(int id)
        {
            //Respawn Player at a clear position
            Debug.WriteLine("RespawnPlayer:" + id);
            var respawnPosition = GetClearPosition(id);
            
            GameHandler.Players[id].SetPosition(respawnPosition);

        }

        public float RandomNumber()
        {
            var random = new Random();
            var min = -1 * (GameHandler.PlayAreaRange);
            var max = GameHandler.PlayAreaRange;
            float rndNum = random.Next(min, max);
            Debug.WriteLine("RandomNumber" + rndNum);
            return rndNum;

        }

        public float4x4 GetClearPosition(int id)
        {
            Debug.WriteLine("GetClearPosition:" + id);
            var clearPosition = float4x4.Identity;
            var rndNum = RandomNumber()*id;
            Debug.WriteLine("GetClearPosition 2");
            clearPosition.M41 = rndNum;
            clearPosition.M42 = rndNum;
            clearPosition.M43 = rndNum;

            foreach (var go in GameHandler.Players)
            {
                if (clearPosition == go.Value.GetPosition())
                {
                    GetClearPosition(id);
                }
            }
            return clearPosition;
        }
    }
}
