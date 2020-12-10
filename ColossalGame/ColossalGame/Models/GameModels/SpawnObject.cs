using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace ColossalGame.Models.GameModels
{
    public class SpawnObject
    {
        public Vector2 InitialPosition { get; set; } = Vector2.Zero;
        public float Radius { get; set; } = 1f;
        public float LinearDamping { get; set; } = 4f;
        public Vector2 InitialVelocity { get; set; } = Vector2.Zero;
        public float Damage { get; set; } = 0f;
        public float InitialDensity { get; set; } = 1f;
        public float InitialFriction { get; set; } = 1f;
        public float InitialRestitution { get; set; } = .3f;
        public float InitialAngle { get; set; } = 0f;
        public float InitialMass { get; set; } = .3f;
        public float Speed { get; set; } = 10f;

        public float InitialHealth { get; set; } = 100f;
    }

    public class PlayerSpawnObject : SpawnObject
    {
        public string Username { get; set; }

        public string PlayerClass { get; set; }

        
        
    }

    public class BulletSpawnObject : SpawnObject
    {
        public PlayerModel Creator { get; set; }

        public string BulletType { get; set; } = "small";

        public new float LinearDamping { get; set; } = 0f;

        public BulletSpawnObject(float angle, float speed, PlayerModel creator, float damage, float radius, Vector2 initialPosition)
        {
            this.InitialVelocity = new Vector2((float)Math.Cos(angle) * speed, (float)-Math.Sin(angle) * speed);
            this.Speed = speed;
            this.InitialAngle = angle;
            this.Creator = creator;
            this.Damage = damage;
            this.Radius = radius;
            this.InitialPosition = initialPosition;
        }
    }

    public class EnemySpawnObject : SpawnObject
    {
        public string EnemyType { get; set; } = "alien_tick";


    }
}
