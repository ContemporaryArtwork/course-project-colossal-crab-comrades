using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.GameModels
{
    public class ExportModel
    {
        public float XPos { get; set; }
        public float YPos { get; set; }
    }

    public class PlayerExportModel : ExportModel
    {
        public string Username { get; set; }
    }

    public class BulletExportModel : ExportModel
    {
        public string BulletType { get; set; }
    }
}
