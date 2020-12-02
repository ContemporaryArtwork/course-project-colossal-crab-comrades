using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ColossalGame.Models;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision.Shapes;
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

        /// <summary>
        ///     Lower bound on milliseconds per world step, can be higher if inputs are sufficiently high
        /// </summary>
        private const double TickRate = 50.0;

        /// <summary>
        /// Amount of milliseconds to wait until publishing the newest game state to clients
        /// </summary>
        private const double PublishRate = 30.0;

        /// <summary>
        /// The ratio of meters in the physics engine to pixels in the game world, i.e. a conversion factor of 64 means that 1 meter in engine is 64 pixels
        /// </summary>
        private const float ConversionFactor = 64.0f;

        

        /// <summary>
        ///     Dictionary of usernames to PlayerModels.
        /// </summary>
        private ConcurrentDictionary<string, Body> PlayerDictionary { get; } = new ConcurrentDictionary<string, Body>();

        /// <summary>
        ///     List of non-player GameObjectModels
        /// </summary>
        private ConcurrentQueue<GameObjectModel> ObjectList { get; } = new ConcurrentQueue<GameObjectModel>();

        /// <summary>
        ///     Queue of Player Actions
        /// </summary>
        private readonly ConcurrentDictionary<string,Vector2> _movementDictionary = new ConcurrentDictionary<string, Vector2>();

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

        private System.Threading.Timer _worldTimer;

        private System.Threading.Timer _publishTimer;

        /// <summary>
        ///     Constructor for GameLogic class
        /// </summary>
        /// <param name="ls">LoginService of server</param>
        /// <param name="us">UserService of server</param>
        public GameLogic(LoginService ls, UserService us)
        {
            _ls = ls;
            _us = us;

            SetupWorld();
            Start();
        }

        /// <summary>
        /// Resets listeners for publishing the state
        /// </summary>
        public void ClearEh()
        {
            //TODO: Change the names of event listener stuff to better reflect what's actually happening
            RaiseCustomEvent = null;
        }

        private void SetupWorld()
        {
            float widthInMeters = 1024*1.5f / ConversionFactor;
            float heightInMeters = 1024*1.5f / ConversionFactor;
            Vector2 lowerLeftCorner = new Vector2(-widthInMeters, -heightInMeters);
            Vector2 lowerRightCorner = new Vector2(widthInMeters, -heightInMeters);
            Vector2 upperLeftCorner = new Vector2(-widthInMeters, heightInMeters);
            Vector2 upperRightCorner = new Vector2(widthInMeters, heightInMeters);
            var edge = _world.CreateBody();
            edge.SetRestitution(0f);
            edge.CreateEdge(lowerLeftCorner, lowerRightCorner);
            edge.CreateEdge(lowerRightCorner, upperRightCorner);
            edge.CreateEdge(upperRightCorner, upperLeftCorner);
            edge.CreateEdge(upperLeftCorner, lowerLeftCorner);
            

        }

        
        

        private Vector2 ConvertMovementActionToVector2(MovementAction action)
        {
            if (!PlayerDictionary.ContainsKey(action.Username)) throw new Exception("Player must be spawned first!");
            var pm = PlayerDictionary.GetValueOrDefault(action.Username);
            if (pm == null)
            {
                //THIS SHOULD NOT BE ABLE TO HAPPEN!!!
                throw new Exception("NULL VALUE IN DICTIONARY");
            }
            float linearImpulseForce = 15f;
            float movementRate = linearImpulseForce/2;

            

            Vector2 desiredVelocity;
            float leftHorizontalVelocity = Math.Max(pm.LinearVelocity.X - movementRate, -linearImpulseForce);
            float rightHorizontalVelocity = Math.Min(pm.LinearVelocity.X + movementRate, linearImpulseForce);
            float upVerticalVelocity = Math.Max(pm.LinearVelocity.Y - movementRate, -linearImpulseForce);
            float downVerticalVelocity = Math.Min(pm.LinearVelocity.Y + movementRate, linearImpulseForce);

            switch (action.Direction)
            {
                case EDirection.Down:
                    desiredVelocity = new Vector2(0, downVerticalVelocity);
                    break;
                case EDirection.Up:
                    desiredVelocity = new Vector2(0, upVerticalVelocity);
                    break;
                case EDirection.Left:
                    
                    desiredVelocity = new Vector2(leftHorizontalVelocity, 0);
                    break;
                case EDirection.Right:
                    
                    desiredVelocity = new Vector2(rightHorizontalVelocity, 0);
                    break;
                case EDirection.UpLeft:
                    desiredVelocity = new Vector2(leftHorizontalVelocity, upVerticalVelocity);
                    break;
                case EDirection.UpRight:
                    desiredVelocity = new Vector2(rightHorizontalVelocity, upVerticalVelocity);
                    break;
                case EDirection.DownLeft:
                    desiredVelocity = new Vector2(leftHorizontalVelocity, downVerticalVelocity);
                    break;
                case EDirection.DownRight:
                    desiredVelocity = new Vector2(rightHorizontalVelocity, downVerticalVelocity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Vector2 velChange = desiredVelocity - pm.LinearVelocity;
            Vector2 impulse = pm.Mass * velChange;
            return impulse;
        }

        public bool IsPlayerSpawned(string username)
        {
            return PlayerDictionary.ContainsKey(username);
        }

        private readonly World _world = new World(Vector2.Zero);

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

            Body pm = new Body();
            pm.CreateCircle(.4f, 1f, playerPosition);
            
            //Can do all the cool physics stuff
            pm.BodyType = BodyType.Dynamic;
            //Bounciness?
            pm.SetRestitution(0f);
            //Friction for touching other bodies
            pm.SetFriction(1f);
            //Just your standard mass
            pm.Mass = 1f;
            //Friction for moving around in space
            pm.LinearDamping = 10f;

            SpinWait.SpinUntil(() => !_world.IsLocked);
            _world.Add(pm);

            
            
            //PlayerDictionary.Add(username, pm);
            PlayerDictionary[username] = pm;
        }

        private void SpawnBullet(float angle,float xPos, float yPos)
        {
            

            Vector2 ballPosition = new Vector2(xPos, yPos);

            var bullet = new Body {BodyType = BodyType.Dynamic};

            var bulletFixture = bullet.CreateCircle(.3f, 1);
            
            bullet.SetRestitution(0.3f);
            bullet.SetFriction(.3f);
            bullet.Mass = .1f;
            bullet.IsBullet = true;

            const float magnitude = 20f;
            var bulletForce = new Vector2((float)Math.Cos(angle)*magnitude,(float)Math.Sin(angle)*magnitude);

            SpinWait.SpinUntil(() => !_world.IsLocked);
            _world.Add(bullet);
            SpinWait.SpinUntil(() => !_world.IsLocked);
            bullet.ApplyForce(bulletForce,bullet.WorldCenter);

            

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
        ///     Add action to server action queue
        /// </summary>
        /// <param name="action">Action to be added</param>
        public void AddActionToQueue(AUserAction action)
        {
            
            if (PlayerDictionary.TryGetValue(action.Username, out var pm) && action is MovementAction m)
            {
                var impulse = ConvertMovementActionToVector2(m);

                SpinWait.SpinUntil(() => !_world.IsLocked);
                pm.ApplyLinearImpulse(impulse, pm.WorldCenter); 
                
            }

            /*
            if (action is MovementAction m)
            {
                _movementDictionary.TryAdd(action.Username, ConvertMovementActionToVector2(m));
            }*/
        }

        /// <summary>
        ///     Will eventually run all user actions and simulate all environmental objects and AI
        /// </summary>
        private void simulateOneServerTick()
        {
            while (PlayerDespawnQueue.Count != 0)
            {
                var p = PlayerDespawnQueue.Dequeue();
                Body pm;
                PlayerDictionary.TryGetValue(p, out pm);
                _world.Remove(pm);
                var successRemove = PlayerDictionary.TryRemove(p,out _);
                
                //Console.WriteLine(successRemove);
            }

            while (PlayerSpawnQueue.Count != 0)
            {
                var p = PlayerSpawnQueue.Dequeue();
                //PlayerDictionary.Add(p.Username, p);
                SpawnPlayer(p.Username,p.XPos,p.YPos);
            }

            foreach (string username in _movementDictionary.Keys)
            {
                _movementDictionary.TryGetValue(username, out var impulse);
                if (PlayerDictionary.TryGetValue(username, out var pm))
                {
                     pm.ApplyLinearImpulse(impulse, pm.WorldCenter);
                }

                _movementDictionary.TryRemove(username, out _);

            }



            //TODO Handle non-player objects
        }

        /// <summary>
        ///     Returns the current game state
        /// </summary>
        /// <returns></returns>
        public (ConcurrentQueue<GameObjectModel>, ConcurrentDictionary<string, Body>) GetState()
        {
            return (ObjectList, PlayerDictionary);
        }

        public static (ConcurrentQueue<GameObjectModel>, ConcurrentDictionary<string, PlayerModel>) GetStatePM(ConcurrentDictionary<string,Body> playerDictionary,ConcurrentQueue<GameObjectModel> objectList)
        {
            
            var returnDictionary = new ConcurrentDictionary<string, PlayerModel>();
            
            
            Parallel.ForEach(playerDictionary, (pair) =>
            {
                var (name, body) = pair;
                var tempPlayerModel = new PlayerModel
                {
                    Username = name,
                    XPos = body.WorldCenter.X * ConversionFactor,
                    YPos = body.WorldCenter.Y * ConversionFactor
                };

                returnDictionary[name] = tempPlayerModel;
            });

            
            return (objectList, returnDictionary);
        }

        /// <summary>
        ///     Starts the server tick processing thread
        /// </summary>
        private void Start()
        {
            //Old method:
            //var instanceCaller = new Thread(RunWorld);
            //var instanceCaller2 = new Thread(StartPublishing);
            //instanceCaller.Start();
            //instanceCaller2.Start();

            //New method (using timers) (more efficient!)
            //Start world stepping
            _worldTimer = new System.Threading.Timer(o => StepWorld(), null,0 , (int)TickRate);
            //Start publishing states
            _publishTimer = new System.Threading.Timer(o => PublishState(), null, 0, (int)PublishRate);

            
            //Thread.Sleep(Timeout.Infinite);
            
        }

        /// <summary>
        ///     Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void StepWorld()
        {
            
            
            var a = new SolverIterations {PositionIterations = 3, VelocityIterations = 8};
            //dt = fraction of steps per second i.e. 50 milliseconds per step has a dt of 50/1000 or 1/20 or every second 20 steps
            //lock because sometimes world stepping will take too long
            lock (_world)
            {
                _world.Step((float) TickRate / 1000f, ref a);
            }
        }



        

       



        /// <summary>
        ///     Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            //Console.WriteLine("Publish: " + DateTime.Now.Second);
            Console.WriteLine("publish");
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
        ///     Add a new player to the spawn queue
        /// </summary>
        /// <param name="username">Username to spawn. Must exist in the user database</param>
        /// <param name="xPos">X Position to spawn at. Default is 0</param>
        /// <param name="yPos">Y Position to spawn at. Default is 0</param>
        public void AddPlayerToSpawnQueue(string username, float xPos = 0f, float yPos = 0f)
        {
            
            if (string.IsNullOrEmpty(username)) throw new Exception("Username is null");
            /*
            var pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;*/
            SpawnPlayer(username);
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
        public CustomEventArgs(ConcurrentQueue<GameObjectModel> objectList, ConcurrentDictionary<string, Body> playerDict)
        {
            ObjectList = objectList;
            PlayerDict = playerDict;
        }

        public ConcurrentQueue<GameObjectModel> ObjectList { get; set; }

        public ConcurrentDictionary<string, Body> PlayerDict { get; set; }
    }
}