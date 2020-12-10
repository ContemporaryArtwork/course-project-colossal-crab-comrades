using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Models.GameModels
{
    public class PlayerModel : GameObjectModel
    {
        public float Health { get; set; } = 100f;
        public string Username { get; set; }

        public PlayerModel(Body b) : base(b)
        {
        }

        public PlayerExportModel Export()
        {
            var retVal = new PlayerExportModel {XPos = this.XPos, YPos = this.YPos, Username = this.Username, Radius = this.Radius, Health = this.Health};
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
