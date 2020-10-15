using ColossalGame.Helpers;
using ColossalGame.Models.DTO;
using ColossalGame.Models.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs
{
    public class GameDataHub : Hub<IGameDataClient>, IGameDataMessageTypes
    {
        public async Task ChangeWeapon(string message)
        {
            await Clients.All.ReceiveString("This was your message: " + message);
        }

        public async Task ExitGame(string message)
        {
            await Clients.All.ReceiveString("This was your message: " + message);
        }

        public async Task FireWeapon(string message)
        {
            await Clients.All.ReceiveString("This was your message: " + message);
        }

        public async Task SendPositionUpdates(string message)
        {
            await Clients.All.ReceiveString("This was your message: " + message);
        }
    }
}
