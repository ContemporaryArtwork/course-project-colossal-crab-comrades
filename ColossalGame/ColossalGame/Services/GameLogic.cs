using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColossalGame.Models;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using Vector2 = tainicom.Aether.Physics2D.Common.Vector2;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        private static readonly object _serverLock = new object();

        /// <summary>
        ///     Login service to keep track of user's login status
        /// </summary>
        private readonly LoginService _ls;

        /// <summary>
        ///     User service to check if users exist
        /// </summary>
        private readonly UserService _us;

        /// <summary>
        ///     boolean representing whether the server thread should continue processing new states
        /// </summary>
        private bool KeepGoing = true;

        private DateTime lastTick = DateTime.Now;

        /// <summary>
        ///     variable that keeps track of the current tick
        /// </summary>
        private int tickCounter;

        /// <summary>
        ///     Amount of seconds per tick
        /// </summary>
        private readonly double tickRate = 50.0;

        private readonly double publishRate = 30.0;

        /// <summary>
        ///     Constructor for GameLogic class
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
        ///     Dictionary of usernames to PlayerModels.
        /// </summary>
        private ConcurrentDictionary<string, Body> PlayerDictionary { get; } = new ConcurrentDictionary<string, Body>();

        /// <summary>
        ///     List of non-player GameObjectModels
        /// </summary>
        private List<GameObjectModel> ObjectList { get; } = new List<GameObjectModel>();

        /// <summary>
        ///     Queue of Player Actions
        /// </summary>
        private ConcurrentQueue<AUserAction> ActionQueue { get; } = new ConcurrentQueue<AUserAction>();

        /// <summary>
        ///     Queue of players to spawn in the next tick
        /// </summary>
        private Queue<PlayerModel> PlayerSpawnQueue { get; } = new Queue<PlayerModel>();

        /// <summary>
        ///     Queue of players to despawn in the next tick
        /// </summary>
        private Queue<string> PlayerDespawnQueue { get; } = new Queue<string>();

        /// <summary>
        ///     Event handler to publish new server states
        /// </summary>
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public void ClearEh()
        {
            RaiseCustomEvent = null;
        }


        /// <summary>
        ///     Void method which handles actions. Currently only handles movement actions
        /// </summary>
        /// <param name="action">Object representing various user actions. Currently only MovementAction will have functionality</param>
        private void HandleAction(Object action)
        {
            
            if (action is MovementAction m)
            {
                if (!PlayerDictionary.ContainsKey(m.Username)) throw new Exception("Player must be spawned first!");
                var pm = PlayerDictionary.GetValueOrDefault(m.Username);
                if (pm == null)
                {
                    //THIS SHOULD NOT BE ABLE TO HAPPEN!!!
                    throw new Exception("NULL VALUE IN DICTIONARY");
                }

                float linearImpulseForce = 10f;
                switch (m.Direction)
                {
                    case EDirection.Down:
                        pm.ApplyLinearImpulse(new Vector2(0, linearImpulseForce),pm.WorldCenter);
                        break;
                    case EDirection.Up:
                        pm.ApplyLinearImpulse(new Vector2(0, -linearImpulseForce), pm.WorldCenter);
                        break;
                    case EDirection.Left:
                        pm.ApplyLinearImpulse(new Vector2(-linearImpulseForce, 0), pm.WorldCenter);
                        break;
                    case EDirection.Right:
                        pm.ApplyLinearImpulse(new Vector2(linearImpulseForce, 0), pm.WorldCenter);
                        break;
                    case EDirection.UpLeft:
                        pm.ApplyLinearImpulse(new Vector2(-linearImpulseForce/2, -linearImpulseForce / 2), pm.WorldCenter);
                        break;
                    case EDirection.UpRight:
                        pm.ApplyLinearImpulse(new Vector2(linearImpulseForce / 2, -linearImpulseForce / 2), pm.WorldCenter);
                        break;
                    case EDirection.DownLeft:
                        pm.ApplyLinearImpulse(new Vector2(-linearImpulseForce / 2, linearImpulseForce / 2), pm.WorldCenter);
                        break;
                    case EDirection.DownRight:
                        pm.ApplyLinearImpulse(new Vector2(linearImpulseForce / 2, linearImpulseForce / 2), pm.WorldCenter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //PlayerDictionary.Add(m.Username, pm);
                PlayerDictionary[m.Username] = pm;
            }

            else if (action is ShootingAction s)
            {
                if (!PlayerDictionary.ContainsKey(s.Username)) throw new Exception("Player must be spawned first!");
            }
        }

        public bool IsPlayerSpawned(string username)
        {
            return PlayerDictionary.ContainsKey(username);
        }

        private World _world = new World(Vector2.Zero);

        /// <summary>
        ///     Spawns a player based on input username, subject to change.
        /// </summary>
        /// <param name="username">Username of desired user to spawn</param>
        /// <param name="xPos">Desired x position</param>
        /// <param name="yPos">Desired y position</param>
        private void SpawnPlayer(string username, float xPos = 0f, float yPos = 0f)
        {
            if (string.IsNullOrEmpty(username)) return;
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            Vector2 playerPosition = new Vector2(xPos, yPos);

            var pm = _world.CreateCircle(1f, 1f, playerPosition);
            pm.BodyType = BodyType.Dynamic;
            pm.SetRestitution(0.3f);
            pm.SetFriction(1f);
            //pm.Mass = .001f;
            //pm.LinearDamping = 1f;

            
            
            //PlayerDictionary.Add(username, pm);
            PlayerDictionary[username] = pm;
        }

        private void SpawnBall(float xPos = 0f, float yPos = 0f)
        {
            

            Vector2 ballPosition = new Vector2(xPos, yPos);

            var pm = _world.CreateCircle(.5f, 1f, ballPosition);
            pm.BodyType = BodyType.Dynamic;
            pm.SetRestitution(0.3f);
            pm.SetFriction(.3f);
            pm.Mass = .1f;
            pm.LinearDamping = 1f;

        }

        /// <summary>
        ///     Despawn a player
        /// </summary>
        /// <param name="username">Username of player to despawn</param>
        public void AddPlayerToDespawnQueue(string username)
        {

            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();
            //PlayerDictionary.Remove(username);
            PlayerDespawnQueue.Enqueue(username);
        }

        /// <summary>
        ///     Will eventually run all user actions and simulate all environmental objects and AI
        /// </summary>
        private void simulateOneServerTick()
        {
            var somethingChanged = false;
            while (PlayerDespawnQueue.Count != 0)
            {
                var p = PlayerDespawnQueue.Dequeue();
                var successRemove = PlayerDictionary.TryRemove(p,out _);
                
                //Console.WriteLine(successRemove);
                somethingChanged = true;
            }

            while (PlayerSpawnQueue.Count != 0)
            {
                var p = PlayerSpawnQueue.Dequeue();
                //PlayerDictionary.Add(p.Username, p);
                SpawnPlayer(p.Username,p.XPos,p.YPos);
                somethingChanged = true;
            }

            while (ActionQueue.Count != 0)
            {
                //Thread t = new Thread(new ParameterizedThreadStart(HandleAction));
                //t.Start(ActionQueue.Dequeue());
                ActionQueue.TryDequeue(out var action);
                HandleAction(action);
                    
                somethingChanged = true;
            }



            //TODO Handle non-player objects
        }

        /// <summary>
        ///     Returns the current game state
        /// </summary>
        /// <returns></returns>
        public (List<GameObjectModel>, ConcurrentDictionary<string, Body>) GetState()
        {
            return (ObjectList, PlayerDictionary);
        }

        public static (List<GameObjectModel>, Dictionary<string, PlayerModel>) GetStatePM(ConcurrentDictionary<string,Body> playerDictionary,List<GameObjectModel> objectList)
        {
            float conversionFactor = 4.0f;
            Dictionary<string,PlayerModel> oPD = new Dictionary<string, PlayerModel>();
            
            var localDict = playerDictionary;
            foreach (string p in localDict.Keys)
            {
                var temp = playerDictionary[p];
                PlayerModel tempPlayerModel = new PlayerModel();
                tempPlayerModel.Username = p;
                tempPlayerModel.XPos = temp.GetWorldPoint(Vector2.Zero).X*conversionFactor;
                
                tempPlayerModel.YPos = temp.GetWorldPoint(Vector2.Zero).Y*conversionFactor;
                oPD[p] = tempPlayerModel;
            }
            return (objectList, oPD);
        }

        /// <summary>
        ///     Starts the server tick processing thread
        /// </summary>
        private void Start()
        {
            var instanceCaller = new Thread(
                RunServer);
            
            var instanceCaller2 = new Thread(
                StartPublishing);
            instanceCaller.Start();
            instanceCaller2.Start();
        }

        /// <summary>
        ///     Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void RunServer()
        {
            
            var ts = new TimeSpan();
            while (KeepGoing)
            {
                    
                //lastTick = DateTime.Now;
                // Console.WriteLine("GameLogic: " + DateTime.Now.Second);
                // Console.WriteLine("ts: " + ts.Milliseconds);
                simulateOneServerTick();
                var a = new SolverIterations {PositionIterations = 3, VelocityIterations = 8};
                _world.Step((float)1/(float)tickRate,ref a);
                ts = DateTime.Now - lastTick;

                
                    
                //PublishState();
                tickCounter++;
                
                Thread.Sleep((int)tickRate);
            }
            
        }



        public void StartPublishing()
        {
            while (true)
            {
                PublishState();
                Thread.Sleep((int)publishRate);
            }

        }

       



        /// <summary>
        ///     Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            //Console.WriteLine("Publish: " + DateTime.Now.Second);
            
            var e = new CustomEventArgs(ObjectList, PlayerDictionary);
            OnRaiseCustomEvent(e);
        }

        /// <summary>
        ///     Tells RaiseCustomEvent to publish event to all subscribers
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
        ///     Add action to server action queue
        /// </summary>
        /// <param name="action">Action to be added</param>
        public void AddActionToQueue(AUserAction action)
        {
           ActionQueue.Enqueue(action);
        }

        /// <summary>
        ///     Add a new player to the spawn queue
        /// </summary>
        /// <param name="username">Username to spawn. Must exist in the user database</param>
        /// <param name="xPos">X Position to spawn at. Default is 0</param>
        /// <param name="yPos">Y Position to spawn at. Default is 0</param>
        public void AddPlayerToSpawnQueue(string username, float xPos = 0f, float yPos = 0f)
        {
            
            if (string.IsNullOrEmpty(username)) throw new Exception("Username is null");
            var pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;
            PlayerSpawnQueue.Enqueue(pm);
        }

        /// <summary>
        ///     Stop the server thread from running new states
        /// </summary>
        public void StopServer()
        {
            KeepGoing = false;
        }

        /// <summary>
        ///     Handles all other game objects, such as projectiles and NPCs
        /// </summary>
        public void handleGameObjects()
        {
        }
    }

    /// <summary>
    ///     Custom EventArgs class which helps publish new state to subscribers
    /// </summary>
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(List<GameObjectModel> objectList, ConcurrentDictionary<string, Body> playerDict)
        {
            ObjectList = objectList;
            PlayerDict = playerDict;
        }

        public List<GameObjectModel> ObjectList { get; set; }

        public ConcurrentDictionary<string, Body> PlayerDict { get; set; }
    }
}