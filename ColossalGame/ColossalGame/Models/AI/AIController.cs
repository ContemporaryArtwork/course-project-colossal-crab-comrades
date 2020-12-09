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

        public void SpawnOne(string enemyType,float innerRadius,float outerRadius, ref ConcurrentQueue<SpawnObject> spawnQueue)
        {
            EnemySpawnObject enemySpawn = new EnemySpawnObject();
            //Set enemy spawn using MATH POWAH
            //https://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus
            var rand = new Random();

            //theta = random.uniform(0,2*pi)
            float theta = (float) rand.NextDouble() * (float) Math.PI * 2f;
            //A = 2/(r_max*r_max - r_min*r_min)
            float A = 2 / (outerRadius * outerRadius - innerRadius * innerRadius);
            //r = sqrt(2*random.uniform(0,1)/A + r_min*r_min)
            float r = (float) Math.Sqrt(
                2 * rand.NextDouble() / (double) A + (double) innerRadius * (double) outerRadius);

            var spawnPosition = new Vector2(r*(float)Math.Cos(theta),r*(float)Math.Sin(theta));
            enemySpawn.InitialPosition = spawnPosition;

            switch (enemyType)
            {
                case "alien_tick":
                    enemySpawn.EnemyType = "alien_tick";
                    enemySpawn.Radius = .25f;
                    enemySpawn.InitialRestitution = 0f;
                    enemySpawn.Speed = .2f;
                    enemySpawn.Damage = 1f;
                    break;
                default:
                    break;
            }

            spawnQueue.Enqueue(enemySpawn);
        }

        public void SpawnWave(int waveSize,float innerRadius, float outerRadius, ref ConcurrentQueue<SpawnObject> spawnQueue)
        {
            var tempSQ = spawnQueue;
            Parallel.For(0, waveSize, (i =>
            {
                SpawnOne("alien_tick", innerRadius, outerRadius, ref tempSQ);
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
