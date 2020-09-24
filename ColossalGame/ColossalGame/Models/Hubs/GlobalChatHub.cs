using System.Threading.Tasks;
using ColossalGame.Models.DTO;
using ColossalGame.Models.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using ColossalGame.Helpers;

namespace ColossalGame.Models.Hubs
{
    public class GlobalChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(GlobalChatMessage message)
        {
            await Clients.All.ReceiveMessage(new GlobalChatMessageDTO { 
                type=Enums.Actions.RECEIVED_MESSAGE.ToString("g"),
                message=message});
        }
    }
}