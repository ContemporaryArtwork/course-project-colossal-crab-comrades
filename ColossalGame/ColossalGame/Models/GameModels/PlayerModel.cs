using ColossalGame.Models.AI;
using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Models.GameModels
{
    public class PlayerModel : GameObjectModel
    {
        public float Health { get; set; } = 100f;

        public float MaxHealth { get; set; } = 100f;
        public string Username { get; set; }

        public string PlayerClass { get; set; }

        public float Exp { get; set; } = 0;

        public int Level { get; set; } = 1;

        public PlayerModel(Body b) : base(b)
        {
        }

        public void AddExp(AIController.EnemyStrength strength)
        {
            float exp = 0;
            switch (strength)
            {
                case AIController.EnemyStrength.Easy:
                    exp = 100;
                    break;
                case AIController.EnemyStrength.EasyMedium:
                    exp = 200;
                    break;
                case AIController.EnemyStrength.Medium:
                    exp = 400;
                    break;
                case AIController.EnemyStrength.Hard:
                    exp = 800;
                    break;
                case AIController.EnemyStrength.VeryHard:
                    exp = 1600;
                    break;
                default:
                    break;
            }
            Exp += exp;
            if (Exp >= Level * 500)
            {
                Exp = 0;
                LevelUp();
            }
        }

        public float FireRate { get; set; } = 200f;
        public void LevelUp()
        {
            Level++;
            Damage = Damage * 1.01f;
            MaxHealth = MaxHealth * 1.01f;
            Speed = Speed * 1.01f;
        }

        public void RegenerateHealth(float percent)
        {
            Health = Health * (1 + percent);
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

        public PlayerExportModel Export()
        {
            var retVal = new PlayerExportModel {XPos = this.XPos, YPos = this.YPos, Username = this.Username, Radius = this.Radius, Health = this.Health, PlayerClass = this.PlayerClass};
            return retVal;
        }

        public void Hurt(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Dead = true;
            }
        }

        public bool Dead { get; set; }
    }
}
