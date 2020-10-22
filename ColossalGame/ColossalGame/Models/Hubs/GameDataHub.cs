using ColossalGame.Helpers;
using ColossalGame.Models.DTO;
using ColossalGame.Models.Hubs.Clients;
using ColossalGame.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs
{
    public class GameDataHub : Hub<IGameDataClient>, IGameDataMessageTypes
    {
        private readonly Interpolator _interpolator;

        public GameDataHub(Interpolator interpolator)
        {
            _interpolator = interpolator;
        }

        public GameDataHub()
        {
            //Method only exists for Test Client for GameDataHub. Need to find way to simulate interpolator working.
        }

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

        public async Task SendMovement(MovementAction movementAction)
        {
            bool res = false;
            

            if (_interpolator != null)
            {
                res = _interpolator.ParseAction(movementAction);
            }

            var responseString = res ? "Action accepted by interpolator" :
                                        "Action rejected by interpolator. Action sent too close to previous action.";
            await Clients.Caller.ReceiveString(responseString);
        }
        
    }
}
