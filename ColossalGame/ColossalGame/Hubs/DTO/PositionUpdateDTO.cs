using ColossalGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.DTO
{
    public class PositionUpdateDTO : ASignalRDTO
    {
        public List<GameObjectModel> ObjectList { get; set; }

        public Dictionary<string, PlayerModel> PlayerDict { get; set; }
    }
}
