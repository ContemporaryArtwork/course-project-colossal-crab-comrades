using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ColossalGame.Helpers;
using ColossalGame.Models;
using ColossalGame.Hubs.DTO;

namespace ColossalGame.Hubs.Chat
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