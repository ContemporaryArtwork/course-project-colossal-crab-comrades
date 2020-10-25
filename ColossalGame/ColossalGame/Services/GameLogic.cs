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
        private bool KeepGoing = true;
        /// <summary>
        /// variable that keeps track of the current tick
        /// </summary>
        private int tickCounter = 0;
        /// <summary>
        /// Amount of seconds per tick
        /// </summary>
        private double tickRate = 1000.0;

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

        public void ClearEh()
        {
            this.RaiseCustomEvent = null;
        }
        /// <summary>
        /// Queue of Player Actions
        /// </summary>
        private Queue<AUserAction> ActionQueue { get; set; } = new Queue<AUserAction>();
        /// <summary>
        /// Queue of players to spawn in the next tick
        /// </summary>
        private Queue<PlayerModel> PlayerSpawnQueue { get; set; } = new Queue<PlayerModel>();

        /// <summary>
        /// Queue of players to despawn in the next tick
        /// </summary>
        private Queue<string> PlayerDespawnQueue { get; set; } = new Queue<string>();

        /// <summary>
        /// Constructor for GameLogic class
        /// </summary>
        /// <param name="ls">LoginService of server</param>
        /// <param name="us">UserService of server</param>
        public GameLogic(LoginService ls, UserService us)
        {
            _ls = ls;
            _us = us;
            //TODO: Separate constructor and Start method
            Start();
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

                //PlayerDictionary.Add(m.Username, pm);
                PlayerDictionary[m.Username] = pm;
            }
        }

        public Boolean IsPlayerSpawned(string username)
        {
            return PlayerDictionary.ContainsKey(username);
        }

        /// <summary>
        /// Spawns a player based on input username, subject to change.
        /// </summary>
        /// <param name="username">Username of desired user to spawn</param>
        /// <param name="xPos">Desired x position</param>
        /// <param name="yPos">Desired y position</param>
        private void SpawnPlayer(string username, double xPos = 0.0, double yPos = 0.0)
        {
            if (string.IsNullOrEmpty(username)) return;
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            var pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;

            //PlayerDictionary.Add(username, pm);
            PlayerDictionary[username] = pm;
        }

        /// <summary>
        /// Despawn a player
        /// </summary>
        /// <param name="username">Username of player to despawn</param>
        public void AddPlayerToDespawnQueue(string username)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();
            //PlayerDictionary.Remove(username);
            PlayerDespawnQueue.Enqueue(username);
        }

        /// <summary>
        /// Will eventually run all user actions and simulate all environmental objects and AI
        /// </summary>
        private void simulateOneServerTick()
        {
            bool somethingChanged = false;
            while (PlayerDespawnQueue.Count != 0)
            {
                var p = PlayerDespawnQueue.Dequeue();
                PlayerDictionary.Remove(p);
                somethingChanged = true;
            }

            while (PlayerSpawnQueue.Count != 0)
            {
                var p = PlayerSpawnQueue.Dequeue();
                //PlayerDictionary.Add(p.Username, p);
                PlayerDictionary[p.Username] = p;
                somethingChanged = true;
            }

            while (ActionQueue.Count != 0)
            {
                HandleAction(ActionQueue.Dequeue());
                somethingChanged = true;
            }

            if (somethingChanged) PublishState();
            



            //TODO Handle non-player objects

        }

        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <returns></returns>
        public (List<GameObjectModel>, Dictionary<string, PlayerModel>) GetState()
        {
            return (ObjectList, PlayerDictionary);
        }

        /// <summary>
        /// Starts the server tick processing thread
        /// </summary>
        private void Start()
        {
            var instanceCaller = new Thread(
                RunServer);
            instanceCaller.Start();
        }

        private static readonly object _serverLock = new object();
        private DateTime lastTick = DateTime.Now;
        /// <summary>
        /// Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void RunServer()
        {
                TimeSpan ts = new TimeSpan();
                while (KeepGoing)
                {
                    

                    if (ts.TotalMilliseconds >= tickRate)
                    {
                        lastTick = DateTime.Now;
                        Console.WriteLine("GameLogic: " + DateTime.Now.Second);
                        Console.WriteLine("ts: " + ts.Milliseconds);
                        simulateOneServerTick();

                        tickCounter++;
                        
                    }

                    ts = DateTime.Now - lastTick;


                }
            
        }



        /// <summary>
        /// Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            
            Console.WriteLine("Publish: "+DateTime.Now.Second);
            var (returnedList, returnedDictionary) = GetState();
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

        /// <summary>
        /// Add action to server action queue
        /// </summary>
        /// <param name="action">Action to be added</param>
        public void AddActionToQueue(AUserAction action)
        {
            ActionQueue.Enqueue(action);
        }

        /// <summary>
        /// Add a new player to the spawn queue
        /// </summary>
        /// <param name="username">Username to spawn. Must exist in the user database</param>
        /// <param name="xPos">X Position to spawn at. Default is 0</param>
        /// <param name="yPos">Y Position to spawn at. Default is 0</param>
        public void AddPlayerToSpawnQueue(string username, double xPos = 0.0, double yPos = 0.0)
        {
            PlayerModel pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;
            PlayerSpawnQueue.Enqueue(pm);
        }

        /// <summary>
        /// Stop the server thread from running new states
        /// </summary>
        public void StopServer()
        {
            KeepGoing = false;
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