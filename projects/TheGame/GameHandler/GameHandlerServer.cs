using System;
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
                            var explo = new Explosion(_gameHandler, go1.Value.GetPosition());

                            // Inform other Players
                            var data = new DataPacketObjectSpawn
                            {
                                UserID = go1.Value.GetId(),
                                ObjectID = explo.GetId(),
                                ObjectType = (int)GameHandler.GameEntities.geExplosion,
                                ObjectVelocity = 0,
                                ObjectPosition = explo.GetPositionVector(),
                                ObjectRotationX = new float3(0, 0, 0),
                                ObjectRotationY = new float3(0, 0, 0),
                                ObjectRotationZ = new float3(0, 0, 0)
                            };

                            var packet = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data };
                            _gameHandler.Mediator.AddToSendingBuffer(packet, false);

                            _gameHandler.Explosions.Add(explo.GetId(), explo);
                            _gameHandler.AudioExplosion.Play();

                            // Kill both players
                            _gameHandler._gameHandlerServer.RespawnPlayer(go1.Value.GetId());
                            _gameHandler._gameHandlerServer.RespawnPlayer(go2.Value.GetId());
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
                            var data1 = new DataPacketObjectUpdate
                            {
                                UserID = bullet.Value.GetOwnerId(),
                                ObjectID = bullet.Value.GetId(),
                                ObjectType = (int) GameHandler.GameEntities.geBullet,
                                ObjectRemoved = true
                            };

                            var packet1 = new DataPacket { PacketType = DataPacketTypes.ObjectUpdate, Packet = data1 };
                            _gameHandler.Mediator.AddToSendingBuffer(packet1, false);

                            _gameHandler.Players[player.Value.GetId()].SetLife(-10);
                            var newHealth = _gameHandler.Players[player.Value.GetId()].GetLife();

                            // Inform specific player!
                            if (newHealth <= 0)
                            {
                                var explo = new Explosion(_gameHandler, player.Value.GetPosition());

                                // Inform other Players
                                var data2 = new DataPacketObjectSpawn
                                {
                                    UserID = player.Value.GetId(),
                                    ObjectID = explo.GetId(),
                                    ObjectType = (int)GameHandler.GameEntities.geExplosion,
                                    ObjectVelocity = 0,
                                    ObjectPosition = explo.GetPositionVector(),
                                    ObjectRotationX = new float3(0, 0, 0),
                                    ObjectRotationY = new float3(0, 0, 0),
                                    ObjectRotationZ = new float3(0, 0, 0)
                                };

                                var packet2 = new DataPacket { PacketType = DataPacketTypes.ObjectSpawn, Packet = data2 };
                                _gameHandler.Mediator.AddToSendingBuffer(packet2, false);

                                _gameHandler.Explosions.Add(explo.GetId(), explo);
                                _gameHandler.AudioExplosion.Play();

                                _gameHandler._gameHandlerServer.RespawnPlayer(0);
                            }
                            else
                            {
                                if (player.Value.GetId() != _gameHandler.UserID)
                                {
                                    var data = new DataPacketPlayerUpdate
                                    {
                                        UserID = player.Value.GetId(),
                                        PlayerActive = true,
                                        PlayerHealth = newHealth,
                                        PlayerVelocity = 0,
                                        PlayerPosition = new float3(0, 0, 0),
                                        PlayerRotationX = new float3(0, 0, 0),
                                        PlayerRotationY = new float3(0, 0, 0),
                                        PlayerRotationZ = new float3(0, 0, 0)
                                    };

                                    var packet = new DataPacket { PacketType = DataPacketTypes.PlayerUpdate, Packet = data };
                                    _gameHandler.Mediator.AddToSendingBuffer(packet, false);
                                }
                            }

                            // GameHandler.Players[_ownerId].SetScore();

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
                        var data1 = new DataPacketObjectUpdate
                            {
                                UserID = player.Value.GetId(),
                                ObjectID = health.Value.GetId(),
                                ObjectType = (int) GameHandler.GameEntities.geHealthItem,
                                ObjectRemoved = true
                            };

                        var packet1 = new DataPacket {PacketType = DataPacketTypes.ObjectUpdate, Packet = data1};
                        _gameHandler.Mediator.AddToSendingBuffer(packet1, false);

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
