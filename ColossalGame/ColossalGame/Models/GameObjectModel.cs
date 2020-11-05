using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models
{
    public class GameObjectModel
    {
        public float YPos { get; set; }
        public float XPos { get; set; }
    }

    public class BulletModel : GameObjectModel
    {
        public Angle Angle { get; set; }
        public double Speed { get; set; }
        public double Range { get; set; }
        public double TickRate { get; set; }

        public void updatePosition()
        {

        }
    }
}
