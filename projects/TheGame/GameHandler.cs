﻿using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TheGame
{
    /// <summary>
    ///     Most general game logic. Maintains game states and updates them.
    /// </summary>
    internal class GameHandler
    {
        /// <summary>
        ///Disctionarires mit allen Items und Playern
        /// </summary>
        public static Dictionary<int, HealthItem> HealthItems;
        public static Dictionary<int, Bullet> Bullets;
        public static Dictionary<int, Player> Players;

        

        public static Dictionary<int, Explosion> Explosions;


        public static List<int> RemoveBullets;
        public static List<int> RemovePlayers;
        public static List<int> RemoveHealthItems;
        public static List<int> RemoveExplosions;

        /// <summary>
        ///State Object, contains the current State the Game is in
        /// </summary>
        internal GameState GameState { get; set; }

        internal int UserID { get; set; }

        private readonly Mediator _mediator;

        /// <summary>
        ///     RenderContext
        /// </summary>
        private readonly RenderContext _rc;

        private float4x4 _camMatrix;
        private int _playerId;
    
        internal GameHandler(RenderContext rc, Mediator mediator)
        {
            //pass RenderContext
            _rc = rc;
            _mediator = mediator;

            HealthItems = new Dictionary<int, HealthItem>();
            Bullets = new Dictionary<int, Bullet>();
            Players = new Dictionary<int, Player>();
            Explosions = new Dictionary<int, Explosion>();
            RemoveBullets = new List<int>();
            RemovePlayers = new List<int>();
            RemoveHealthItems = new List<int>();
            RemoveExplosions = new List<int>();

            GameState = new GameState(GameState.State.StartMenu);

            _camMatrix = float4x4.Identity;

            StartGame();
            

            this.AddNewPlayer();

            Debug.WriteLine("_playerId: "+_playerId);

        }

        internal void Update()
        {
            foreach (var go in HealthItems)
                go.Value.Update();
            foreach (var go in Bullets)
                go.Value.Update();
            foreach (var go in Explosions)
                go.Value.Update();
            foreach (var go in Players)
            {
                if (go.Key != _playerId)
                    go.Value.Update();
            }
            Players[_playerId].PlayerInput();
            Players[_playerId].Update();
            _camMatrix = Players[_playerId].GetCamMatrix();

            foreach (var removePlayer in RemovePlayers)
            {
                Players.Remove(removePlayer);
            }
             foreach (var removeItem in RemoveHealthItems)
            {
                RemoveHealthItems.Remove(removeItem);
            }
            foreach (int removeBullet in RemoveBullets)
            {
                Bullets.Remove(removeBullet);
            }
            foreach (int removeExplosion in RemoveExplosions)
            {
                Explosions.Remove(removeExplosion);
            }
            RemovePlayers.Clear();
            RemoveHealthItems.Clear();
            RemoveBullets.Clear();
            RemoveExplosions.Clear();
        }

        internal void Render()
        {
            foreach (var go in HealthItems)
                go.Value.RenderUpdate(_rc, _camMatrix);
            foreach (var go in Bullets)
                go.Value.RenderUpdate(_rc, _camMatrix);
            foreach (var go in Explosions)
                go.Value.RenderUpdate(_rc, _camMatrix);
            foreach (var go in Players)
            {
                if (go.Key != _playerId)
                {
                    go.Value.RenderUpdate(_rc, _camMatrix);
                   // Debug.WriteLine("Playerrender: "+ go.Value.GetId());
                }
            }
            Players[_playerId].RenderUpdate(_rc,_camMatrix);
           // Debug.WriteLine("Playerrenderlast: " + Players[_playerId].GetId());
        }

        internal void StartGame()
        {
            var p = new Player(_mediator, _rc, 100, float4x4.Identity, 0, 0, _mediator.UserID);
            Players.Add(p.GetId(), p);
            _playerId = p.GetId();
        }
        internal void AddNewPlayer()
        {
            var p = new Player(_mediator, _rc, 100, float4x4.Identity * float4x4.CreateTranslation(600, 0, 0), 0, 0,11);
            Players.Add(p.GetId(), p);
            p = new Player(_mediator, _rc, 100, float4x4.Identity * float4x4.CreateTranslation(300f, 0, 0), 0, 0, 22);
            Players.Add(p.GetId(), p);
            p = new Player(_mediator, _rc, 100, float4x4.Identity * float4x4.CreateTranslation(0, 300f, 0), 0, 0,33);
            Players.Add(p.GetId(), p);
            p = new Player(_mediator, _rc, 100, float4x4.Identity * float4x4.CreateTranslation(0, 0, -300f), 0, 0,44);
            Players.Add(p.GetId(), p);
        }
    }
}