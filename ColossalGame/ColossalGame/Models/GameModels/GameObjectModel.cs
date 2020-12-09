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

        public Vector2 Position
        {
            get => ObjectBody.WorldCenter;
            set => ObjectBody.SetTransform(value, ObjectBody.Rotation);
        }
        
        public float LinearDamping
        {
            get => ObjectBody.LinearDamping; 
            set => ObjectBody.LinearDamping=value;
        }

        public float Damage { get; set; } = 0f;
        public float Speed { get; set; } = 10f;
    }

    public class BulletModel : GameObjectModel
    {
        
        public string BulletType { get; set; }

        public new float Damage { get; set; } = 10f;


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
        public new float Damage { get; set; } = 5f;

        public new float Speed { get; set; } = 5f;

        public float Health { get; set; } = 100f;
        

        private PlayerModel _closestPlayer;

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

            _closestPlayer = player;
        }

        public void MoveTowardsClosestPlayer()
        {
            if (_closestPlayer == default(PlayerModel))
            {
                //Should we throw an exception here?
                //I choose not to for simplicity of iterating through AI
                return;
            }

            var playerPos = _closestPlayer.ObjectBody.WorldCenter;
            var ourPos = ObjectBody.WorldCenter;
            var directionalVector =  playerPos - ourPos;
            directionalVector.Normalize();
            Vector2.Distance(ref playerPos,ref ourPos,out var distance);
            if (distance > 1f)
            {
                ObjectBody.ApplyLinearImpulse(directionalVector * this.Speed, ObjectBody.WorldCenter);
            }
            else
            {
                ObjectBody.ApplyLinearImpulse(directionalVector * this.Speed, ObjectBody.WorldCenter);
            }
        }

        public EnemyExportModel Export()
        {
            var retVal = new EnemyExportModel() { XPos = this.XPos, YPos = this.YPos, EnemyType = this.EnemyType, ID = this.ID, Radius = this.Radius, Health = this.Health };
            return retVal;
        }

        public EnemyModel(Body b) : base(b)
        {
        }
    }

}
