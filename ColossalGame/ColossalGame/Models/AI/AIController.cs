using ColossalGame.Models.GameModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

namespace ColossalGame.Models.AI
{
    
    public class AIController
    {
        private ConcurrentQueue<int> EnemyList { get; set; } = new ConcurrentQueue<int>();
        



        public AIController()
        {
            
        }

        public void Recalculate(ConcurrentDictionary<string, PlayerModel> playerDictionary, ConcurrentDictionary<int,GameObjectModel> objectDictionary)
        {

            Parallel.ForEach(EnemyList, id =>
            {
                objectDictionary.TryGetValue(id, out var gameObject);
                if (gameObject is EnemyModel enemy)
                {
                    enemy.SearchForClosestPlayer(playerDictionary);
                }
            });
        }

        public void Step(ConcurrentDictionary<int,GameObjectModel> objectDictionary)
        {
            Parallel.ForEach(EnemyList, id =>
            {
                objectDictionary.TryGetValue(id, out var gameObject);
                if (gameObject is EnemyModel enemy)
                {
                    enemy.MoveTowardsClosestPlayer();
                }

            });
        }

        public enum WaveTypes
        {
            Ring,
            Corners,
            InsideOut
        }

        public enum EnemyStrength
        {
            Easy,
            EasyMedium,
            Medium,
            Hard,
            VeryHard
        }

        public enum WaveSize
        {
            Small,
            Medium,
            Large,
            XtraLarge
        }

        public void SpawnOneTick(EnemyStrength enemyStrength, ref ConcurrentQueue<SpawnObject> spawnQueue, WaveTypes waveType, float innerRadius=1600f/64f, float outerRadius=2000f/64f,float sideLength=4000f/64f)
        {
            EnemySpawnObject enemySpawn = new EnemySpawnObject();
            var rand = new Random();
            if (waveType == WaveTypes.Ring)
            {
                //Set enemy spawn using MATH POWAH
                //https://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus
                
                //theta = random.uniform(0,2*pi)
                float theta = (float) rand.NextDouble() * (float) Math.PI * 2f;
                //A = 2/(r_max*r_max - r_min*r_min)
                float A = 2 / (outerRadius * outerRadius - innerRadius * innerRadius);
                //r = sqrt(2*random.uniform(0,1)/A + r_min*r_min)
                float r = (float) Math.Sqrt(
                    2 * rand.NextDouble() / (double) A + (double) innerRadius * (double) outerRadius);
                var spawnPosition = new Vector2(r * (float) Math.Cos(theta), r * (float) Math.Sin(theta));
                enemySpawn.InitialPosition = spawnPosition;
            }
            else if (waveType == WaveTypes.Corners)
            {
                var entropy = (float)rand.NextDouble();
                if (entropy >= .5)
                {
                    enemySpawn.InitialPosition = new Vector2(sideLength/2+entropy,sideLength/2+entropy);
                }
                else
                {
                    enemySpawn.InitialPosition = new Vector2(-sideLength/2+entropy,-sideLength/2+entropy);
                }
            }
            else if (waveType == WaveTypes.InsideOut)
            {
                enemySpawn.InitialPosition = Vector2.Zero;
            }

            var enemyType = "alien_tick";
            enemySpawn.Radius = .4f;
            enemySpawn.InitialRestitution = 0f;
            switch (enemyStrength)
            {
                case EnemyStrength.Easy:
                    enemySpawn.EnemyType = enemyType;
                    enemySpawn.Speed = .4f;
                    enemySpawn.Damage = 2f;
                    enemySpawn.InitialHealth = 10f;
                    break;
                case EnemyStrength.EasyMedium:
                    enemySpawn.EnemyType = enemyType;
                    enemySpawn.Speed = .5f;
                    enemySpawn.Damage = 5f;
                    enemySpawn.InitialHealth = 10f;
                    break;
                case EnemyStrength.Medium:
                    enemySpawn.EnemyType = enemyType;
                    enemySpawn.Speed = .6f;
                    enemySpawn.Damage = 10f;
                    enemySpawn.InitialHealth = 20f;
                    break;
                case EnemyStrength.Hard:
                    enemySpawn.EnemyType = enemyType;
                    enemySpawn.Speed = .65f;
                    enemySpawn.Damage = 25f;
                    enemySpawn.InitialHealth = 25f;
                    break;
                case EnemyStrength.VeryHard:
                    enemySpawn.EnemyType = enemyType;
                    enemySpawn.Speed = .75f;
                    enemySpawn.Damage = 35f;
                    enemySpawn.InitialHealth = 30f;
                    break;
            }
            

            spawnQueue.Enqueue(enemySpawn);
        }

        public void Reset()
        {
            EnemyList.Clear();
        }

        public void SpawnWave(EnemyStrength enemyStrength,WaveSize waveSize,int players,float innerRadius, float outerRadius, ref ConcurrentQueue<SpawnObject> spawnQueue)
        {
            var tempSQ = spawnQueue;
            int waveCount;
            switch (waveSize)
            {
                case WaveSize.Small:
                    waveCount = 5 * players;
                    break;
                case WaveSize.Medium:
                    waveCount = 10 * players;
                    break;
                case WaveSize.Large:
                    waveCount = 20 * players;
                    break;
                case WaveSize.XtraLarge:
                    waveCount = 40 * players;
                    break;
                default:
                    waveCount = 10 * players;
                    break;
            }
            Parallel.For(0, waveCount, (i =>
            {
                SpawnOneTick(enemyStrength, ref tempSQ, WaveTypes.Corners);
            }));
            spawnQueue = tempSQ;
        }

        public void Register(EnemyModel enemy, ref ConcurrentDictionary<int,GameObjectModel> objectDictionary)
        {
            if (EnemyList.Contains(enemy.ID)) throw new Exception("Tried to add register enemy when it was already");
            EnemyList.Enqueue(enemy.ID);
        }

        public void Deregister(int id)
        {
            EnemyList = new ConcurrentQueue<int>(EnemyList.Where((i => i!=id)));
        }

        
        
    }
}
