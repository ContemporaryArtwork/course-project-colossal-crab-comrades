using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.GameModels
{
    public abstract class GameObjectExportModel
    {
        public float XPos { get; set; }
        public float YPos { get; set; }
    }

    public class PlayerExportModel : GameObjectExportModel
    {
        public string Username { get; set; }
    }

    public class BulletExportModel : GameObjectExportModel
    {
        public string BulletType { get; set; }
        public int ID { get; set; }
    }
}
