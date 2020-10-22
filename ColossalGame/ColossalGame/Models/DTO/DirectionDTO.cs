using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ColossalGame.Models.DTO
{
    public class DirectionDTO : ASignalRDTO
    {
        public EDirection Direction { get; set; }
    }
}
