using System;
using System.Collections.Generic;
using System.Threading;
using ColossalGame.Models;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        /// <summary>
        /// Login service to keep track of user's login status
        /// </summary>
        private readonly LoginService _ls;
        /// <summary>
        /// User service to check if users exist
        /// </summary>
        private readonly UserService _us;
        /// <summary>
        /// boolean representing whether the server thread should continue processing new states
        /// </summary>
        private readonly bool KeepGoing = true;
        /// <summary>
        /// variable that keeps track of the current tick
        /// </summary>
        private int tickCounter = 0;
        /// <summary>
        /// Amount of seconds per tick
        /// </summary>
        private readonly int tickRate = 50;

        /// <summary>
        /// Dictionary of usernames to PlayerModels.
        /// </summary>
        private Dictionary<string, PlayerModel> PlayerDictionary { get; } = new Dictionary<string, PlayerModel>();
        /// <summary>
        /// List of non-player GameObjectModels
        /// </summary>
        private List<GameObjectModel> ObjectList { get; set; } = new List<GameObjectModel>();
        /// <summary>
        /// Event handler to publish new server states
        /// </summary>
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        /// <summary>
        /// Constructor for GameLogic class
        /// </summary>
        /// <param name="ls">LoginService of server</param>
        /// <param name="us">UserService of server</param>
        /// <param name="startServer">Boolean representing whether server should immediately start tick processing thread on construction</param>
        public GameLogic(LoginService ls, UserService us, bool startServer)
        {
            _ls = ls;
            _us = us;
            if (startServer) Start();
        }

        
        /// <summary>
        /// Void method which handles actions. Currently only handles movement actions
        /// </summary>
        /// <param name="action">Object representing various user actions. Currently only MovementAction will have functionality</param>
        private void HandleAction(AUserAction action)
        {

            if (action is MovementAction m)
            {
                var pm = PlayerDictionary.GetValueOrDefault(m.Username);
                if (pm == null) throw new Exception("Player must be spawned first!");
                if (m.Direction == EDirection.Down)
                    pm.YPos -= 1;
                else if (m.Direction == EDirection.Up)
                    pm.YPos += 1;
                else if (m.Direction == EDirection.Left)
                    pm.XPos -= 1;
                else if (m.Direction == EDirection.Right) pm.XPos += 1;

                PlayerDictionary.Add(m.Username, pm);
            }
        }

        /// <summary>
        /// Spawns a player based on input username, subject to change.
        /// </summary>
        /// <param name="username">Username of desired user to spawn</param>
        /// <param name="xPos">Desired x position</param>
        /// <param name="yPos">Desired y position</param>
        public void SpawnPlayer(string username, double xPos = 0.0, double yPos = 0.0)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            var pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;

            PlayerDictionary.Add(username, pm);
        }

        /// <summary>
        /// Despawn a player
        /// </summary>
        /// <param name="username">Username of player to despawn</param>
        public void DespawnPlayer(string username)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();
            PlayerDictionary.Remove(username);
        }

        /// <summary>
        /// Will eventually run all user actions and simulate all environmental objects and AI
        /// </summary>
        public void simulateOneServerTick()
        {
            //TODO
        }

        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <returns></returns>
        public (List<GameObjectModel>, Dictionary<string, PlayerModel>) getState()
        {
            //TODO
            return (null, null);
        }

        /// <summary>
        /// Starts the server tick processing thread
        /// </summary>
        public void Start()
        {
            var instanceCaller = new Thread(
                RunServer);
            instanceCaller.Start();
        }

        /// <summary>
        /// Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void RunServer()
        {
            while (KeepGoing)
            {
                Thread.Sleep(tickRate);
                simulateOneServerTick();
                tickCounter++;
                //TODO: Add logic dictating when to publish the state instead of after every tick
                PublishState();
            }
        }

        /// <summary>
        /// Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            var (returnedList, returnedDictionary) = getState();
            var e = new CustomEventArgs(returnedList, returnedDictionary);
            OnRaiseCustomEvent(e);
        }

        /// <summary>
        /// Tells RaiseCustomEvent to publish event to all subscribers
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            RaiseCustomEvent?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Custom EventArgs class which helps publish new state to subscribers
    /// </summary>
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(List<GameObjectModel> objectList, Dictionary<string, PlayerModel> playerDict)
        {
            ObjectList = objectList;
            PlayerDict = playerDict;
        }

        public List<GameObjectModel> ObjectList { get; set; }

        public Dictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}