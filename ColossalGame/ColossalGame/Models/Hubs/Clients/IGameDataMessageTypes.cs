using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs.Clients
{
    public interface IGameDataMessageTypes
    {
        Task SendPositionUpdates(string message);

        Task ExitGame(string message);

        Task ChangeWeapon(string message);

        Task FireWeapon(string message);


    }
}
