using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColossalGame.Models.Hubs.Clients
{
    public interface IGameDataMessageTypes
    {
        Task SendMovement(object movementAction);

        Task ExitGame(string message);

        Task ChangeWeapon(string message);

        Task FireWeapon(string message);


    }
}
