using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.DTO
{
    public class TokenMessageDTO: ASignalRDTO
    {
        public string Token { get; set; }
    }
}
