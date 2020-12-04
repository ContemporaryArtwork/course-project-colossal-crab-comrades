using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ColossalGame.Models;
using ColossalGame.Models.GameModels;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using Vector2 = tainicom.Aether.Physics2D.Common.Vector2;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        /// <summary>
        ///     Login service to keep track of user's login status
        /// </summary>
        private readonly LoginService _ls;

        /// <summary>
        ///     User service to check if users exist
        /// </summary>
        private readonly UserService _us;

        /// <summary>
        ///     Lower bound on milliseconds per world step, can be higher if inputs are sufficiently high
        /// </summary>
        private const double TickRate = 30.0;

        /// <summary>
        /// Amount of milliseconds to wait until publishing the newest game state to clients
        /// </summary>
        private const double PublishRate = 30.0;

        /// <summary>
        /// The ratio of meters in the physics engine to pixels in the game world, i.e. a conversion factor of 64 means that 1 meter in engine is 64 pixels
        /// </summary>
        private static float ConversionFactor = 64.0f;

        
        /// <summary>
        ///     Dictionary of usernames to PlayerModels.
        /// </summary>
        private ConcurrentDictionary<string, PlayerModel> PlayerDictionary { get; } = new ConcurrentDictionary<string, PlayerModel>();

        /// <summary>
        ///     List of non-player GameObjectModels
        /// </summary>
        private ConcurrentDictionary<int,GameObjectModel> ObjectDictionary { get; } = new ConcurrentDictionary<int, GameObjectModel>();


        /// <summary>
        ///     Event handler to publish new server states
        /// </summary>
        public event EventHandler<PublishEvent> Publisher;

        /// <summary>
        /// Object which steps the world every {tickRate} milliseconds
        /// </summary>
        private System.Threading.Timer _worldTimer;

        /// <summary>
        /// Object which publishes the states
        /// </summary>
        private System.Threading.Timer _publishTimer;

        private Mutex _data = new Mutex();

        private Mutex _nextToAccess = new Mutex();

        private Mutex _lowPriority = new Mutex();


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
            Publisher = null;
        }

        /// <summary>
        /// Add boundaries to the world, among other things (TODO!)
        /// </summary>
        private void SetupWorld()
        {
            //TODO: Make the bounds bigger??
            float widthInMeters = 1024*1.5f / ConversionFactor;
            float heightInMeters = 1024*1.5f / ConversionFactor;
            Vector2 lowerLeftCorner = new Vector2(-widthInMeters, -heightInMeters);
            Vector2 lowerRightCorner = new Vector2(widthInMeters, -heightInMeters);
            Vector2 upperLeftCorner = new Vector2(-widthInMeters, heightInMeters);
            Vector2 upperRightCorner = new Vector2(widthInMeters, heightInMeters);
            var edge = _world.CreateBody();
            edge.SetRestitution(1f);
            edge.SetFriction(1f);
            Vertices v = new Vertices();
            v.Add(lowerLeftCorner);
            v.Add(lowerRightCorner);
            v.Add(upperLeftCorner);
            v.Add(upperRightCorner);

            //edge.CreateLoopShape(v);
            
            edge.CreateEdge(lowerLeftCorner, lowerRightCorner);
            edge.CreateEdge(lowerRightCorner, upperRightCorner);
            edge.CreateEdge(upperRightCorner, upperLeftCorner);
            edge.CreateEdge(upperLeftCorner, lowerLeftCorner);
            


        }

        
        

        private Vector2 ConvertMovementActionToVector2(MovementAction action)
        {
            if (!PlayerDictionary.ContainsKey(action.Username)) throw new Exception("Player must be spawned first!");
            var playerModel = PlayerDictionary.GetValueOrDefault(action.Username);
            if (playerModel == null)
            {
                //THIS SHOULD NOT BE ABLE TO HAPPEN!!!
                throw new Exception("NULL VALUE IN DICTIONARY");
            }

            var pm = playerModel.ObjectBody;

            //Start calculations
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

            var playerModel = new PlayerModel(pm);
            playerModel.Username = username;

            pm.Tag = playerModel;
            
            //PlayerDictionary.Add(username, pm);
            PlayerDictionary[username] = playerModel;
        }

        private void SpawnBullet(float angle,Vector2 ballPosition, PlayerModel spawner)
        {
            

            var bullet = new Body {BodyType = BodyType.Dynamic};
            

            var bulletFixture = bullet.CreateCircle(.3f, 1);
            
            bullet.SetRestitution(1f);
            bullet.SetFriction(1f);
            bullet.Mass = .1f;
            bullet.IsBullet = true;

            const float magnitude = 30f;
            var bulletForce = new Vector2((float)Math.Cos(angle)*magnitude,(float)-Math.Sin(angle)*magnitude);
            bullet.ApplyForce(bulletForce, bullet.WorldCenter);

            var bulletModel = new BulletModel(bullet)
            {
                BulletType = "small",//TODO: Make this better somehow?
                Damage = 10f
            };

            bullet.Tag = bulletModel;


            /*bullet.OnCollision += (fixtureA, fixtureB, contact) =>
            {
                //TODO: Handle collisions, right now we just destroy the bullets when they hit things

                if (fixtureA.Tag is PlayerModel a)
                {
                    if (a.Username == spawner.Username)
                    {
                        return false;
                    }
                }

                if (fixtureB.Tag is PlayerModel b)
                {
                    if (b.Username == spawner.Username)
                    {
                        return false;
                    }
                }

                if (fixtureA.Tag is BulletModel bm)
                {
                    SpinWait.SpinUntil(() => !_world.IsLocked);
                    _world.Remove(bm.ObjectBody);
                    ObjectDictionary.Remove(bm.ID, out var value);
                }

                if (fixtureB.Tag is BulletModel bm2)
                {
                    SpinWait.SpinUntil(() => !_world.IsLocked);
                    _world.Remove(bm2.ObjectBody);
                    ObjectDictionary.Remove(bm2.ID, out var value);
                }

                return true;
            };
*/
            Action A = () =>
            {
                _world.Add(bullet);
                bullet.SetTransform(ballPosition, angle);
            };


            LowPriority(A);
                
            

            
            


            
            ObjectDictionary.TryAdd(bulletModel.ID,bulletModel);
            
        }


        /// <summary>
        ///     Despawn a player
        /// </summary>
        /// <param name="username">Username of player to despawn</param>
        public void DespawnPlayer(string username)
        {
            throw new NotImplementedException();
            //if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            //TODO: Implement this
        }

        private bool LowPriority(Action action)
        {
            /*if (_lowPriority.WaitOne(50))
            {
                _nextToAccess.WaitOne();
                _data.WaitOne();
                _nextToAccess.ReleaseMutex();
                action();
                _data.ReleaseMutex();
                _lowPriority.ReleaseMutex();
                return true;
            }
            return false
             */

            _nextToAccess.WaitOne();
            _nextToAccess.ReleaseMutex();
            action();
            return true;

            


        }

        private void HighPriorityLock()
        {
            _nextToAccess.WaitOne();
            
            
        }

        private void HighPriorityUnlock()
        {
            _nextToAccess.ReleaseMutex();
        }





        /// <summary>
        ///     Add action to server action queue
        /// </summary>
        /// <param name="action">Action to be added</param>
        public void HandleAction(AUserAction action)
        {
            var spawned = PlayerDictionary.TryGetValue(action.Username, out var playerModel);
            var playerBody = playerModel.ObjectBody;
            if (!spawned)
            {
                throw new Exception("Player must be spawned first!");
            }

            switch (action)
            {
                case MovementAction m:
                {
                    var impulse = ConvertMovementActionToVector2(m);


                        void A()
                        {
                            playerBody.ApplyLinearImpulse(impulse, playerBody.WorldCenter);
                        }


                        LowPriority(A);

                        break;
                }
                case ShootingAction s:
                {

                    
                    SpawnBullet(s.Angle,playerBody.WorldCenter,playerModel);
                    
                    break;
                }
                default:
                    throw new ArgumentException("Unsupported Action Type");
            }

            
        }


        /// <summary>
        ///     Returns the current game state
        /// </summary>
        /// <returns></returns>
        public (ConcurrentDictionary<int,GameObjectModel>, ConcurrentDictionary<string, PlayerModel>) GetState()
        {
            return (ObjectDictionary, PlayerDictionary);
        }

        public static (ConcurrentQueue<object>, ConcurrentDictionary<string, PlayerExportModel>) GetStatePM(ConcurrentDictionary<string,PlayerModel> playerDictionary,ConcurrentDictionary<int, GameObjectModel> objectDict)
        {
            
            var returnDictionary = new ConcurrentDictionary<string, PlayerExportModel>();
            var gameObjectQueue = new ConcurrentQueue<object>();
            
            Parallel.ForEach(playerDictionary, (pair) =>
            {
                var (name, playerModel) = pair;
                
                
                returnDictionary[name] = playerModel.Export();
            });

            Parallel.ForEach(objectDict, (pair) =>
            {
                var (id, model) = pair;

                if(model is BulletModel b)
                {
                    gameObjectQueue.Enqueue(b.Export());
                }
                else if(model is EnemyModel e)
                {
                    //gameObjectQueue.Enqueue(e.Export()); TODO
                }
                else
                {
                    throw new Exception("When casting, to exported model the model type was not supported.");
                }

                
            });


            return (gameObjectQueue, returnDictionary);
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

            
            
        }

        

        /// <summary>
        ///     Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void StepWorld()
        {
            
            
            var a = new SolverIterations {PositionIterations = 2, VelocityIterations = 4};
            //dt = fraction of steps per second i.e. 50 milliseconds per step has a dt of 50/1000 or 1/20 or every second 20 steps
            //lock because sometimes world stepping will take too long
            
            HighPriorityLock();
            _world.Step((float) TickRate / 1000f, ref a);
            HighPriorityUnlock();

           
            
        }



        

       



        /// <summary>
        ///     Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            //Console.WriteLine("Publish: " + DateTime.Now.Second);
            //Console.WriteLine("publish");
            var e = new PublishEvent(ObjectDictionary, PlayerDictionary);
            HandlePublish(e);
        }

        /// <summary>
        ///     Tells Publisher to publish event to all subscribers
        /// </summary>
        /// <param name="state"></param>
        protected virtual void HandlePublish(PublishEvent state)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            Publisher?.Invoke(this, state);
        }

        

        /// <summary>
        ///     Add a new player to the spawn queue
        /// </summary>
        /// <param name="username">Username to spawn. Must exist in the user database</param>
        /// <param name="xPos">X Position to spawn at. Default is 0</param>
        /// <param name="yPos">Y Position to spawn at. Default is 0</param>
        public void HandleSpawnPlayer(string username, float xPos = 0f, float yPos = 0f)
        {
            
            if (string.IsNullOrEmpty(username)) throw new Exception("Username is null");
            //TODO: Get rid of this useless method
            SpawnPlayer(username);
        }

        /// <summary>
        ///     Stop the server thread from running new states
        /// </summary>
        public void StopServer()
        {
            //There's pretty much no reason anyone would ever use this?
            _worldTimer.Dispose();
            _publishTimer.Dispose();
        }

        /// <summary>
        ///     Handles all other game objects, such as projectiles and NPCs
        /// </summary>
        public void handleGameObjects()
        {
            //TODO
        }
    }

    /// <summary>
    ///     Custom EventArgs class which helps publish new state to subscribers
    /// </summary>
    public class PublishEvent : EventArgs
    {
        public PublishEvent(ConcurrentDictionary<int, GameObjectModel> objectDict, ConcurrentDictionary<string, PlayerModel> playerDict)
        {
            ObjectDict = objectDict;
            PlayerDict = playerDict;
        }

        public ConcurrentDictionary<int, GameObjectModel> ObjectDict { get; set; }

        public ConcurrentDictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}