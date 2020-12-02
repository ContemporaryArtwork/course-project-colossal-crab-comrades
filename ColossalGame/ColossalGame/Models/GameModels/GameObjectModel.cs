namespace ColossalGame.Models.GameModels
{
    public class GameObjectModel
    {
        public float YPos { get; set; }
        public float XPos { get; set; }
    }

    public class BulletModel : GameObjectModel
    {
        public string BulletType { get; set; }
        public float Radius { get; set; }
        public float Damage { get; set; }
    }

    public class EnemyModel : GameObjectModel
    {
        public string EnemyType { get; set; }
        public float Radius { get; set; }
        public float Damage { get; set; }
    }

}
