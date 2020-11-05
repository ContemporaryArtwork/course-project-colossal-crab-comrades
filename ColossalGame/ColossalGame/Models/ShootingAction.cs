using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models
{
    public class ShootingAction : AUserAction
    {
        public Angle ShootingAngle { get; set; }
        public double Speed { get; set; }
    }
}
