using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models
{
    public enum EDirection
    {
        Down,
        Up,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public class AUserAction
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }

    public class MovementAction : AUserAction
    {
        
        public EDirection Direction { get; set; }
        

    }

    public class SpawnAction: AUserAction
    {
        
    }

    
}
