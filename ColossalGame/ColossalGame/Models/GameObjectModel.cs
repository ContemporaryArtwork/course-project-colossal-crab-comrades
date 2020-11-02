using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models
{
    public class GameObjectModel
    {
        public double YPos { get; set; }
        public double XPos { get; set; }
    }

    public class BulletModel : GameObjectModel
    {
        public Angle Angle { get; set; }
        public double Speed { get; set; }
    }
}
