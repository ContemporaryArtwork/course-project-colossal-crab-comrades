using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColossalGame.Models.AI;
using ColossalGame.Models.Exceptions;
using ColossalGame.Models.GameModels;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.PhysicsLogic;
using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        /// <summary>
        ///     Lower bound on milliseconds per world step, can be higher if inputs are sufficiently high
        /// </summary>
        private const double TickRate = 30.0;

        /// <summary>
        ///     Amount of milliseconds to wait until publishing the newest game state to clients
        /// </summary>
        private const double PublishRate = 10.0;

        /// <summary>
        ///     The ratio of meters in the physics engine to pixels in the game world, i.e. a conversion factor of 64 means that 1
        ///     meter in engine is 64 pixels
        /// </summary>
        private static readonly float ConversionFactor = 64.0f;

        /// <summary>
        ///     Login service to keep track of user's login status
        /// </summary>
        private readonly LoginService _ls;

        /// <summary>
        ///     User service to check if users exist
        /// </summary>
        private readonly UserService _us;

        private World _world = new World(Vector2.Zero);

        private readonly Mutex _data = new Mutex();

        private readonly Mutex _lowPriority = new Mutex();

        private readonly Mutex _nextToAccess = new Mutex();

        /// <summary>
        ///     Object which publishes the states
        /// </summary>
        private System.Threading.Timer _publishTimer;

        /// <summary>
        ///     Object which steps the world every {tickRate} milliseconds
        /// </summary>
        private System.Threading.Timer _worldTimer;

        private readonly ConcurrentQueue<AUserAction> actionQueue = new ConcurrentQueue<AUserAction>();

        private Mutex worldMutex = new Mutex();

        private ConcurrentQueue<SpawnObject> spawnQueue = new ConcurrentQueue<SpawnObject>();

        private AIController aiController = new AIController();

        private ConcurrentDictionary<string,int> deathCounterDictionary = new ConcurrentDictionary<string, int>();


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
        ///     Dictionary of usernames to PlayerModels.
        /// </summary>
        private ConcurrentDictionary<string, PlayerModel> PlayerDictionary { get; } =
            new ConcurrentDictionary<string, PlayerModel>();

        /// <summary>
        ///     List of non-player GameObjectModels
        /// </summary>
        private ConcurrentDictionary<int, GameObjectModel> _objectDictionary =
            new ConcurrentDictionary<int, GameObjectModel>();


        /// <summary>
        ///     Event handler to publish new server states
        /// </summary>
        public event EventHandler<PublishEvent> Publisher;

        private ConcurrentQueue<GameObjectModel> cleanupQueue = new ConcurrentQueue<GameObjectModel>();

        /// <summary>
        ///     Resets listeners for publishing the state
        /// </summary>
        public void ClearEh()
        {
            //TODO: Change the names of event listener stuff to better reflect what's actually happening
            Publisher = null;
        }

        /// <summary>
        ///     Add boundaries to the world, among other things (TODO!)
        /// </summary>
        private void SetupWorld()
        {
            //TODO: Make the bounds bigger??
            var widthInMeters = 1024 * 1.5f / ConversionFactor;
            var heightInMeters = 1024 * 1.5f / ConversionFactor;
            var lowerLeftCorner = new Vector2(-widthInMeters, -heightInMeters);
            var lowerRightCorner = new Vector2(widthInMeters, -heightInMeters);
            var upperLeftCorner = new Vector2(-widthInMeters, heightInMeters);
            var upperRightCorner = new Vector2(widthInMeters, heightInMeters);
            var edge = _world.CreateBody();
            edge.Tag = "worldBounds";
            edge.SetRestitution(1f);
            edge.SetFriction(1f);
            var v = new Vertices();
            v.Add(lowerLeftCorner);
            v.Add(lowerRightCorner);
            v.Add(upperLeftCorner);
            v.Add(upperRightCorner);

            //edge.CreateLoopShape(v);

            edge.CreateEdge(lowerLeftCorner, lowerRightCorner);
            edge.CreateEdge(lowerRightCorner, upperRightCorner);
            edge.CreateEdge(upperRightCorner, upperLeftCorner);
            edge.CreateEdge(upperLeftCorner, lowerLeftCorner);

            //TODO: Remove this, debugging only
            
        }


        private Vector2 ConvertMovementActionToVector2(MovementAction action, PlayerModel playerModel)
        {
            

            var playerBody = playerModel.ObjectBody;

            //Start calculations
            var linearImpulseForce = playerModel.Speed;
            var movementRate = linearImpulseForce / 2;


            Vector2 desiredVelocity;
            var leftHorizontalVelocity = Math.Max(playerBody.LinearVelocity.X - movementRate, -linearImpulseForce);
            var rightHorizontalVelocity = Math.Min(playerBody.LinearVelocity.X + movementRate, linearImpulseForce);
            var upVerticalVelocity = Math.Max(playerBody.LinearVelocity.Y - movementRate, -linearImpulseForce);
            var downVerticalVelocity = Math.Min(playerBody.LinearVelocity.Y + movementRate, linearImpulseForce);

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

            var velChange = desiredVelocity - playerBody.LinearVelocity;
            var impulse = playerBody.Mass * velChange;
            return impulse;
        }

        public void Reset()
        {
            Parallel.ForEach(_objectDictionary, ((pair, state) =>
            {
                var (key, value) = pair;
                MarkEntityForDestruction(value);
            }));
            aiController.Reset();
            PlayerDictionary.Clear();
            deathCounterDictionary.Clear();
            _objectDictionary.Clear();
            _world = new World(Vector2.Zero);
        }

        public bool IsPlayerSpawned(string username)
        {
            return PlayerDictionary.ContainsKey(username);
        }

        public bool HasPlayerDied(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new Exception("Username is null");
            if (!deathCounterDictionary.ContainsKey(username)) deathCounterDictionary.TryAdd(username, 0);
            deathCounterDictionary.TryGetValue(username, out var deathCount);
            if (deathCount > 0)
            {
                //throw new Exception("");
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Spawns a player based on input username, subject to change.
        /// </summary>
        /// <param name="username">Username of desired user to spawn</param>
        /// <param name="xPos">Desired x position</param>
        /// <param name="yPos">Desired y position</param>
        private void SpawnPlayer(PlayerSpawnObject playerSpawn)
        {
            var username = playerSpawn.Username;
            
            if (string.IsNullOrEmpty(username)) return;
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            var playerPosition = playerSpawn.InitialPosition;

            var pm = new Body();
            pm.CreateCircle(playerSpawn.Radius, playerSpawn.InitialDensity, playerPosition);

            //Can do all the cool physics stuff
            pm.BodyType = BodyType.Dynamic;
            //Bounciness?
            pm.SetRestitution(playerSpawn.InitialRestitution);
            //Friction for touching other bodies
            pm.SetFriction(playerSpawn.InitialFriction);
            //Just your standard mass
            pm.Mass = playerSpawn.InitialMass;
            //Friction for moving around in space
            pm.LinearDamping = playerSpawn.LinearDamping;

            SpinWait.SpinUntil(() => !_world.IsLocked);
            _world.Add(pm);

            var playerModel = new PlayerModel(pm);
            playerModel.Username = username;
            playerModel.Health = playerSpawn.InitialHealth;
            playerModel.Damage = playerSpawn.Damage;
            pm.Tag = playerModel;

            //PlayerDictionary.Add(username, pm);
            PlayerDictionary[username] = playerModel;
        }

        private void MarkEntityForDestruction(GameObjectModel b)
        {
            var bulletBody = b.ObjectBody;
            if (!_world.BodyList.Contains(bulletBody)) return;
            if (cleanupQueue.Contains(b)) return;
            cleanupQueue.Enqueue(b);
        }

        private void SpawnBullet(BulletSpawnObject bulletSpawn)
        {
            //Load initial values
            var ballPosition = bulletSpawn.InitialPosition;
            var creator = bulletSpawn.Creator;

            //Define body
            var bullet = new Body {BodyType = BodyType.Dynamic};

            var bulletFixture = bullet.CreateCircle(bulletSpawn.Radius, 1);

            bullet.SetRestitution(bulletSpawn.InitialRestitution);
            bullet.SetFriction(bulletSpawn.InitialFriction);
            bullet.Mass = bulletSpawn.InitialMass;
            bullet.IsBullet = true;

            
            bullet.ApplyForce(bulletSpawn.InitialVelocity, bullet.WorldCenter);

            var bulletModel = new BulletModel(bullet)
            {
                BulletType = "small", //TODO: Make this better somehow?
                Damage = 10f
            };

            bullet.Tag = bulletModel;

            
            _world.Add(bullet);
            bullet.SetTransform(ballPosition, bulletSpawn.InitialAngle);
            bulletFixture.OnCollision += (fixtureA, fixtureB, contact) =>
            {
                if (fixtureA.Body.Tag is PlayerModel playerA)
                {
                    if (playerA.Username == creator.Username)
                    {
                        return false;
                    }
                }

                if (fixtureB.Body.Tag is PlayerModel playerB)
                {
                    if (playerB.Username == creator.Username)
                    {
                        return false;
                    }
                }


                if (fixtureA.Body.Tag is BulletModel b1)
                {
                    if (b1.ID == bulletModel.ID)
                    {
                        MarkEntityForDestruction(bulletModel);
                    }
                }

                if (fixtureB.Body.Tag is BulletModel b2)
                {
                    if (b2.ID == bulletModel.ID)
                    {
                        MarkEntityForDestruction(bulletModel);
                    }
                }

                if (fixtureA.Body.Tag is EnemyModel e1)
                {
                    e1.Hurt(bulletModel.Damage);
                    if (e1.Dead) MarkEntityForDestruction(e1);
                }

                if (fixtureB.Body.Tag is EnemyModel e2)
                {
                    e2.Hurt(bulletModel.Damage);
                    if (e2.Dead) MarkEntityForDestruction(e2);
                }



                return false;
                };
            
            _objectDictionary.TryAdd(bulletModel.ID, bulletModel);
        }

        

        private void SpawnEnemy(EnemySpawnObject enemySpawn)
        {
            
            //Define body
            var enemy = new Body { BodyType = BodyType.Dynamic };

            var enemyFixture = enemy.CreateCircle(enemySpawn.Radius, 1);

            enemy.SetRestitution(enemySpawn.InitialRestitution);
            enemy.SetFriction(enemySpawn.InitialFriction);
            enemy.Mass = enemySpawn.InitialMass;
            enemy.LinearDamping = enemySpawn.LinearDamping;



            var enemyModel = new EnemyModel(enemy);
            
            enemyModel.EnemyType = enemySpawn.EnemyType;
            enemyModel.Damage = enemySpawn.Damage;
            enemyModel.Speed = enemySpawn.Speed;
            enemyModel.Health = enemySpawn.InitialHealth;
            

            enemy.Tag = enemyModel;


            _world.Add(enemy);
            enemy.SetTransform(enemySpawn.InitialPosition, enemySpawn.InitialAngle);
            enemy.OnCollision += (fixtureA, fixtureB, contact) =>
            {
                if (fixtureA.Body.Tag.Equals("worldBounds")||fixtureB.Body.Tag.Equals("worldBounds"))
                {
                    return false;
                }

                if (fixtureA.Body.Tag is PlayerModel p1)
                {
                    if (fixtureB.Body.Tag is EnemyModel e && !e.Dead)
                    {
                        p1.Hurt(enemyModel.Damage);
                    }
                    
                    if (p1.Dead)
                    {
                        MarkEntityForDestruction(p1);
                        if (fixtureB.Body.Tag is EnemyModel eB)
                        {
                            eB.ResetClosestPlayer();
                        }
                    }
                }

                if (fixtureB.Body.Tag is PlayerModel p2)
                {
                    if (fixtureA.Body.Tag is EnemyModel e && !e.Dead)
                    {
                        p2.Hurt(enemyModel.Damage);
                    }
                    
                    if (p2.Dead)
                    {
                        MarkEntityForDestruction(p2);
                        if (fixtureA.Body.Tag is EnemyModel eA)
                        {
                            eA.ResetClosestPlayer();
                        }
                    }
                }
                return true;
            };


            _objectDictionary.TryAdd(enemyModel.ID, enemyModel);
            aiController.Register(enemyModel,ref _objectDictionary);
            
            enemyModel.SearchForClosestPlayer(PlayerDictionary);
        }


        /// <summary>
        ///     Deregister a player
        /// </summary>
        /// <param name="username">Username of player to despawn</param>
        public void DespawnPlayer(string username)
        {
            PlayerDictionary.TryGetValue(username, out var playerModel);
            if (playerModel != null)
            {
                cleanupQueue.Enqueue(playerModel);
            }
        }


        public void HandleAction(AUserAction action)
        {
            var spawned = PlayerDictionary.TryGetValue(action.Username, out var playerModel);
            if (!spawned) throw new UnspawnedException("Player must be spawned first!");
            actionQueue.Enqueue(action);
        }

        private void ProcessActionQueue()
        {
            Parallel.For(0, actionQueue.Count, ((i, state) =>
            {
                actionQueue.TryDequeue(out var action);
                if (action != null)
                {
                    RunAction(action);
                }
            }));
            //Parallel.ForEach(actionQueue, RunAction);
            //actionQueue.Clear();
        }

        private void Cleanup()
        {
            while (cleanupQueue.Any())
            {
                cleanupQueue.TryDequeue(out var model);
                if (model is BulletModel b)
                {
                    var bulletBody = b.ObjectBody;
                    if (_world.BodyList.Contains(bulletBody))
                    {
                        
                        _world.Remove(bulletBody);
                        _objectDictionary.TryRemove(b.ID, out var _);
                    }

                    
                }else if (model is EnemyModel e)
                {
                    var enemyBody = e.ObjectBody;
                    if (_world.BodyList.Contains(enemyBody))
                    {
                        _world.Remove(enemyBody);
                        aiController.Deregister(e.ID);
                        _objectDictionary.TryRemove(e.ID, out var _);
                    }
                }else if (model is PlayerModel p)
                {
                    var playerBody = p.ObjectBody;
                    if (_world.BodyList.Contains(playerBody))
                    {
                        _world.Remove(playerBody);
                        PlayerDictionary.TryRemove(p.Username, out var _);
                        deathCounterDictionary.TryGetValue(p.Username, out var value);
                        deathCounterDictionary.TryUpdate(p.Username, value+1, value);
                    }

                    if (PlayerDictionary.IsEmpty)
                    {
                        Reset();
                        SetupWorld();
                        Restart();
                    }
                }
                else
                {
                    throw new Exception("Trying to cleanup something not supported");
                }
            }
            
        }

        private void Spawn()
        {
            
            while(spawnQueue.Any())
            {
                spawnQueue.TryDequeue(out var spawnObj);
                if (spawnObj == null) break;
                if (spawnObj is BulletSpawnObject bulletSpawn)
                {
                    SpawnBullet(bulletSpawn);
                }else if (spawnObj is PlayerSpawnObject playerSpawn)
                {
                    SpawnPlayer(playerSpawn);
                }
                else if (spawnObj is EnemySpawnObject enemySpawn)
                {
                    SpawnEnemy(enemySpawn);
                }
                else
                {
                    throw new ArgumentException("Tried to parse an unsupported Spawn Object");
                }
            }
        }


        /// <summary>
        ///     Add action to server action queue
        /// </summary>
        /// <param name="action">Action to be added</param>
        public void RunAction(AUserAction action)
        {
            var spawned = PlayerDictionary.TryGetValue(action.Username, out var playerModel);

            if (!spawned) return;
            var playerBody = playerModel.ObjectBody;
            switch (action)
            {
                case MovementAction m:
                {
                    
                    var impulse = ConvertMovementActionToVector2(m,playerModel);
                    playerBody.ApplyLinearImpulse(impulse, playerBody.WorldCenter);
                    break;
                }
                case ShootingAction s:
                {
                    PlayerDictionary.TryGetValue(s.Username, out var creator);
                    if (creator==null) throw new Exception("Player not found when spawning bullet");
                    var bulletSpawn = new BulletSpawnObject(s.Angle,20f,creator,10f,.3f,creator.ObjectBody.Position);
                    bulletSpawn.InitialMass = .02f;
                    spawnQueue.Enqueue(bulletSpawn);

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
        public (ConcurrentDictionary<int, GameObjectModel>, ConcurrentDictionary<string, PlayerModel>) GetState()
        {
            return (_objectDictionary, PlayerDictionary);
        }

        public static (ConcurrentQueue<object>, ConcurrentDictionary<string, PlayerExportModel>) GetStatePM(
            ConcurrentDictionary<string, PlayerModel> playerDictionary,
            ConcurrentDictionary<int, GameObjectModel> objectDict)
        {
            var returnDictionary = new ConcurrentDictionary<string, PlayerExportModel>();
            var gameObjectQueue = new ConcurrentQueue<object>();

            Parallel.ForEach(playerDictionary, pair =>
            {
                var (name, playerModel) = pair;


                returnDictionary[name] = playerModel.Export();
            });

            Parallel.ForEach(objectDict, pair =>
            {
                var (id, model) = pair;

                if (model is BulletModel b)
                {
                    gameObjectQueue.Enqueue(b.Export());
                }
                else if (model is EnemyModel e)
                {
                    gameObjectQueue.Enqueue(e.Export()); 
                }
                else
                {
                    throw new Exception("When casting, to exported model the model type was not supported.");
                }
            });

            


            return (gameObjectQueue, returnDictionary);
        }

        private System.Threading.Timer waveTimer;

        private int waveNum = 1;
        private void SpawnWave()
        {
            switch (waveNum)
            {
                case 1:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.Easy, AIController.WaveSize.Large, PlayerDictionary.Count);
                    waveNum++;
                    break;
                case 2:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.Medium, AIController.WaveSize.Large, PlayerDictionary.Count);
                    waveNum++;
                    break;
                case 3:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.Medium, AIController.WaveSize.Large, PlayerDictionary.Count);
                    waveNum++;
                    break;
                case 4:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.Hard, AIController.WaveSize.Medium, PlayerDictionary.Count);
                    waveNum++;
                    break;
                case 5:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.Hard, AIController.WaveSize.Large, PlayerDictionary.Count);
                    waveNum++;
                    break;
                case 6:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.VeryHard, AIController.WaveSize.Large, PlayerDictionary.Count);
                    waveNum++;
                    break;
                default:
                    aiController.SpawnWave(ref spawnQueue, AIController.EnemyStrength.VeryHard, AIController.WaveSize.XtraLarge, PlayerDictionary.Count);
                    break;
            }
            
        }

        private System.Threading.Timer _aiBrainTimer;
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
            _worldTimer = new System.Threading.Timer(o => StepWorld(), null, 0, (int) TickRate);
            //Start publishing states
            _publishTimer = new System.Threading.Timer(o => PublishState(), null, 0, (int) PublishRate);

            _aiBrainTimer = new System.Threading.Timer(o=>StepAiBrain(),null,0,5000);

            waveTimer = new System.Threading.Timer(o=>SpawnWave(),null,0,20*1000);

        }

        private void Restart()
        {
            waveNum = 1;
            _worldTimer.DisposeAsync();
            _publishTimer.DisposeAsync();
            _aiBrainTimer.DisposeAsync();
            waveTimer.DisposeAsync();

            //New method (using timers) (more efficient!)
            //Start world stepping
            _worldTimer = new System.Threading.Timer(o => StepWorld(), null, 0, (int)TickRate);
            //Start publishing states
            _publishTimer = new System.Threading.Timer(o => PublishState(), null, 0, (int)PublishRate);

            _aiBrainTimer = new System.Threading.Timer(o => StepAiBrain(), null, 0, 5000);

            waveTimer = new System.Threading.Timer(o => SpawnWave(), null, 0, 20 * 1000);

        }

        private void StepAiBrain()
        {
            aiController.Recalculate(PlayerDictionary, _objectDictionary);
        }

        private void StepAiMovement()
        {
            aiController.Step(_objectDictionary);
        }


        /// <summary>
        ///     Keeps looping every {tickRate} milliseconds, simulating a new server tick every time
        /// </summary>
        private void StepWorld()
        {
            
            var solverIterations = new SolverIterations {PositionIterations = 2, VelocityIterations = 4};

            //lock because sometimes world stepping will take too long
            lock (_world)
            {
                
                Cleanup();
                Spawn();
                if (PlayerDictionary.IsEmpty) return;
                ProcessActionQueue();
                StepAiMovement();

                //dt = fraction of steps per second i.e. 50 milliseconds per step has a dt of 50/1000 or 1/20 or every second 20 steps
                _world.Step((float)TickRate / 1000f, ref solverIterations);
            }
            
        }


        /// <summary>
        ///     Publishes current state to event handler
        /// </summary>
        private void PublishState()
        {
            //Console.WriteLine("Publish: " + DateTime.Now.Second);
            //Console.WriteLine("publish");
            var e = new PublishEvent(_objectDictionary, PlayerDictionary);
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
            if (!deathCounterDictionary.ContainsKey(username)) deathCounterDictionary.TryAdd(username, 0);
            deathCounterDictionary.TryGetValue(username, out var deathCount);
            if (deathCount > 0)
            {
                //throw new Exception("");
                return;
            }
            else
            {
                //TODO: Get rid of this useless method
                var playerSpawn = new PlayerSpawnObject();
                playerSpawn.Username = username;
                playerSpawn.InitialPosition = new Vector2(xPos, yPos);
                playerSpawn.LinearDamping = 4f;
                playerSpawn.Speed = 20f;
                playerSpawn.Radius = .4f;
                playerSpawn.Damage = 10f;
                playerSpawn.InitialHealth = 100f;
                spawnQueue.Enqueue(playerSpawn);
            }
        }

        public void HandleRespawnPlayer(string username, float xPos = 0f, float yPos = 0f)
        {
            if (string.IsNullOrEmpty(username)) throw new Exception("Username is null");
            //TODO: Get rid of this useless method
            var playerSpawn = new PlayerSpawnObject();
            playerSpawn.Username = username;
            playerSpawn.InitialPosition = new Vector2(xPos, yPos);
            playerSpawn.LinearDamping = 4f;
            playerSpawn.Speed = 20f;
            playerSpawn.Radius = .4f;
            spawnQueue.Enqueue(playerSpawn);
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
        public PublishEvent(ConcurrentDictionary<int, GameObjectModel> objectDict,
            ConcurrentDictionary<string, PlayerModel> playerDict)
        {
            ObjectDict = objectDict;
            PlayerDict = playerDict;
        }

        public ConcurrentDictionary<int, GameObjectModel> ObjectDict { get; set; }

        public ConcurrentDictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}