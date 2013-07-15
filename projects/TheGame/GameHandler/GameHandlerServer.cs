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

        private const int PlayAreaRange = 2000;

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
                            // Inform other players that the bullet should be removed!!
                            var data = new DataPacketObjectUpdate
                            {
                                UserID = bullet.Value.GetOwnerId(),
                                ObjectID = bullet.Value.GetId(),
                                ObjectType = 0,
                                ObjectRemoved = true
                            };

                            var packet = new DataPacket { PacketType = DataPacketTypes.ObjectUpdate, Packet = data };
                            _gameHandler.Mediator.AddToSendingBuffer(packet, true);

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

        public void RespawnPlayer(uint getId)
        {
            if (getId == 0 && _gameHandler.UserID == 0)
            {
                var respawnPosition = RandomPosition();

                while (_gameHandler.Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                {
                    respawnPosition = RandomPosition();
                }

                _gameHandler.Players[getId].SetPosition(respawnPosition);
                _gameHandler.Players[getId].ResetLife();
            }
            else
            {
                // SERVER ACTIVITY!
                var respawnPosition = RandomPosition();

                while (_gameHandler.Players.Any(player => respawnPosition == player.Value.GetPositionVector()))
                {
                    respawnPosition = RandomPosition();
                }

                // send back to user
                var data = new DataPacketPlayerSpawn
                {
                    UserID = getId,
                    Spawn = true,
                    SpawnPosition = respawnPosition
                };

                var packet = new DataPacket { PacketType = DataPacketTypes.PlayerSpawn, Packet = data };
                _gameHandler.Mediator.AddToSendingBuffer(packet, true);

                // reset life
                if (!_gameHandler.Players.ContainsKey(getId))
                {
                    var p = new Player(_gameHandler, float4x4.Identity, 0, getId);
                    _gameHandler.Players.Add(getId, p);
                }

                _gameHandler.Players[getId].SetPosition(respawnPosition);
                _gameHandler.Players[getId].ResetLife();
            }
        }
    }
}
