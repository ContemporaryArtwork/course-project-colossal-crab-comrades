using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Models.GameModels
{
    public class GameObjectModel
    {
        public int ID { get; }
        public Body ObjectBody { get; set; }

        public float ConversionFactor { get; set; } = 64f;

        public float Radius
        {
            get => ObjectBody.FixtureList.First().Shape.Radius * ConversionFactor;
            set => ObjectBody.FixtureList.First().Shape.Radius = value/ConversionFactor;
        }

        public GameObjectModel(Body b)
        {
            ObjectBody = b;
            ID = Counter++;
        }

        public static int Counter { get; set; } = 0;

        public float YPos
        {
            get => ObjectBody.WorldCenter.Y*ConversionFactor;
            set => ObjectBody.SetTransform(new Vector2(XPos,value),ObjectBody.Rotation);
        }

        public float XPos
        {
            get => ObjectBody.WorldCenter.X*ConversionFactor;
            set => ObjectBody.SetTransform(new Vector2(value, YPos), ObjectBody.Rotation);
        }

        /*public GameObjectExportModel Export()
        {
            var retVal = new GameObjectExportModel();
            retVal.XPos = this.XPos;
            retVal.YPos = this.YPos;
            return retVal;
        }*/
    }

    public class BulletModel : GameObjectModel
    {
        
        public string BulletType { get; set; }
        
        public float Damage { get; set; }


        public BulletModel(Body b) : base(b)
        {
        }

        public BulletExportModel Export()
        {
            var retVal = new BulletExportModel { XPos = this.XPos, YPos = this.YPos, BulletType = this.BulletType, ID= this.ID, Radius = this.Radius};
            return retVal;
        }
    }

    public class EnemyModel : GameObjectModel
    {
        public string EnemyType { get; set; }
        public float Damage { get; set; }

        private PlayerModel closestPlayer;

        public void SearchForClosestPlayer(ConcurrentDictionary<string,PlayerModel> playerDictionary)
        {
            if (playerDictionary.IsEmpty) return;
            var ourLocation = ObjectBody.WorldCenter;

            //Simple linear search
            //TODO: Optimize this?
            PlayerModel player = playerDictionary.First().Value;
            float distance = 10000000;
            foreach (var (key,value) in playerDictionary)
            {
                float compareDistance;
                var theirLocation = value.ObjectBody.WorldCenter;
                Vector2.Distance(ref ourLocation, ref theirLocation, out compareDistance);
                if (distance > compareDistance)
                {
                    player = value;
                    distance = compareDistance;
                }
            }

            closestPlayer = player;
        }

        public void MoveTowardsClosestPlayer()
        {
            if (closestPlayer == default(PlayerModel))
            {
                //Should we throw an exception here?
                //I choose not to for simplicity of iterating through AI
                return;
            }

            var directionalVector = ObjectBody.WorldCenter - closestPlayer.ObjectBody.WorldCenter;
            float distanceProportion = .1f;
            ObjectBody.ApplyLinearImpulse(directionalVector*distanceProportion);
        }

        public EnemyExportModel Export()
        {
            var retVal = new EnemyExportModel() { XPos = this.XPos, YPos = this.YPos, EnemyType = this.EnemyType, ID = this.ID, Radius = this.Radius };
            return retVal;
        }

        public EnemyModel(Body b) : base(b)
        {
        }
    }

}
