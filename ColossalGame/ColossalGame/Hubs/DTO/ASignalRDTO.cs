using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.DTO
{
    public abstract class ASignalRDTO : IPayload
    {
        public string type { get; set; }

    }
}
