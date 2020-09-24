using ColossalGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(GlobalChatMessageDTO message);
    }
}
