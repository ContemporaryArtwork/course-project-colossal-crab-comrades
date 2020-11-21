using ColossalGame.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace ColossalGame.Hubs.DTO
{
    public class PositionUpdateDTO : ASignalRDTO
    {
        public ConcurrentQueue<GameObjectModel> ObjectList { get; set; }

        public ConcurrentDictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}
