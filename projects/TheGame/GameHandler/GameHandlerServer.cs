﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine;
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
            SpawnInitialHealthItems();
        }

        public float RandomNumber()
        {
            const int min = -1 * (PlayAreaRange);
            const int max = PlayAreaRange;

            float rndNum = _random.Next(min, max);

            return rndNum;
        }

        public float3 RandomPosition()
        {
            var rndNum1 = RandomNumber();
            var rndNum2 = RandomNumber();
            var rndNum3 = RandomNumber();
            
            return new float3(rndNum1, rndNum2, rndNum3);
        }

        private void SpawnInitialHealthItems()
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnHealthItem();
            }
        }

        public void SpawnHealthItem()
        {
            var hi = new HealthItem(_gameHandler, float4x4.Identity * float4x4.CreateTranslation(RandomPosition()), 0);
            _gameHandler.HealthItems.Add(hi.GetId(), hi);
        }

        internal bool CheckCollision(GameEntity oe1, GameEntity oe2)
        {
            return (oe1.GetPositionVector() - oe2.GetPositionVector()).LengthSquared <=
                   oe1.GetCollisionRadius() * oe1.GetCollisionRadius() +
                   oe2.GetCollisionRadius() * oe2.GetCollisionRadius();
        }

        internal void CheckAllCollision()
        {
            /*foreach (var go in GameHandler.Players)*/
            foreach( var go1 in _gameHandler.Players)
            {
                foreach (var go2 in _gameHandler.Players)
                {

                    if (go1.Value.GetId() != go2.Value.GetId())
                    {
                        if (CheckCollision(go1.Value, go2.Value))
                        {
                            // Kill both players
                            go1.Value.DestroyEnity();
                            go2.Value.DestroyEnity();
                        }
                    }
                }
            }


            foreach (var player in _gameHandler.Players)
            {
                foreach (var bullet in _gameHandler.Bullets)
                {
                    if (player.Value.GetId() != bullet.Value.GetOwnerId())
                    {
                        if (CheckCollision(player.Value, bullet.Value))
                        {
                            bullet.Value.OnCollisionEnter(player.Value.GetId());
                        }   
                    }
                }
            }

            foreach (var player in _gameHandler.Players)
            {
                foreach (var health in _gameHandler.HealthItems)
                {  
                    if (CheckCollision(player.Value, health.Value))
                    {
                       health.Value.OnCollisionEnter(player.Value.GetId());
                    }
                }
            }
        }


        public void Update()
        {
            CheckAllCollision();
        }
    }
}
