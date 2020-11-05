using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Models.DTO
{
    public class PositionUpdateDTO : ASignalRDTO
    {
        public List<GameObjectModel> ObjectList { get; set; }

        public Dictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}
