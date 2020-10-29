using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.DTO
{
    interface IPayload
    {
        public string type { get; set; }
    }
}
