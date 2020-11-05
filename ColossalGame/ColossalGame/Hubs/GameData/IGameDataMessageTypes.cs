using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models;

namespace ColossalGame.Hubs.GameData
{
    public interface IGameDataMessageTypes
    {
        Task SendMovement(MovementAction movementAction);

        Task ExitGame(string message);

        Task ChangeWeapon(string message);

        Task FireWeapon(string message);


    }
}
