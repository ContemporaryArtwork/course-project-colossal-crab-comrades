using ColossalGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs.Clients
{
    public interface IGameDataClient
    {
        Task ReceiveString(string message);

        Task ReceiveToken(string message);
    }
}
