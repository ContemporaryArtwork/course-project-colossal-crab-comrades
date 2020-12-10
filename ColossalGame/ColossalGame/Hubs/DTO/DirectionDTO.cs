using ColossalGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models.GameModels;


namespace ColossalGame.Hubs.DTO
{
    public class DirectionDTO : ASignalRDTO
    {
        public EDirection Direction { get; set; }
    }
}
