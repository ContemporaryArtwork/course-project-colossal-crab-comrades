using ColossalGame.Hubs.DTO;
using ColossalGame.Models;
using ColossalGame.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.GameData
{
    public class GameDataHub : Hub<IGameDataClient>, IGameDataMessageTypes
    {
        private readonly Interpolator _interpolator;
        private readonly GameLogic _gameLogic;
        private readonly LoginService _ls;
        private readonly IHubContext<GameDataHub, IGameDataClient> _hubContext;
        private bool added = false;
        public GameDataHub(Interpolator interpolator, GameLogic gamelogic, LoginService ls, IHubContext<GameDataHub, IGameDataClient> hubContext)
        {
            Console.WriteLine("Constructor called");
            _interpolator = interpolator;
            _gameLogic = gamelogic;
            _gameLogic.ClearEh();
            _gameLogic.RaiseCustomEvent += HandleCustomEvent;
            
            _ls = ls;
            _hubContext = hubContext;
        }

/*        public GameDataHub()
        {
            //Method only exists for Test Client for GameDataHub. Need to find way to simulate interpolator working.
        }*/
        private DateTime lastUpdate = DateTime.Now;
         public async void HandleCustomEvent(object sender, CustomEventArgs e)
         {
             
                 
                 PositionUpdateDTO positionUpdateDTO = new PositionUpdateDTO
                 {
                     type = "RECEIVE_POSITIONS_UPDATE",
                     ObjectList = e.ObjectList,
                     PlayerDict = e.PlayerDict
                 };
                 await _hubContext.Clients.All.ReceiveMessage(positionUpdateDTO);
                 lastUpdate = DateTime.Now;
             
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

        public async Task TempLogin(string username, string password)
        {
            _ls.DeleteUser(username);
            _ls.SignUp(username, password);
            string token = _ls.SignIn(username, password);


            await Clients.Caller.ReceiveToken(new TokenMessageDTO() { type="RECEIVE_TOKEN", Token=token });
        }
        
        public async Task SendMovement(MovementAction movementAction)
        {
            bool res = false;
            
            
            if (_interpolator != null)
            {
                try
                {
                    if (!_gameLogic.IsPlayerSpawned(movementAction.Username)) //Should Deprecate this in favor of requiring the player to call the SpawnPlayer action.
                    {
                        _gameLogic.AddPlayerToSpawnQueue(movementAction.Username);
                    }
                    
                    res = _interpolator.ParseAction(movementAction);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }

            var responseString = res ? "Action accepted by interpolator" :
                                        "Action rejected by interpolator. Action sent too close to previous action.";
            await Clients.Caller.ReceiveString(responseString);
        }

        public async Task SpawnPlayer(SpawnAction spawnAction)
        {
            bool res;
            string responseString;

            if (_interpolator != null)
            {
                try
                {
                    res = _gameLogic.IsPlayerSpawned(spawnAction.Username);
                    if (!res)
                    {
                        _gameLogic.AddPlayerToSpawnQueue(spawnAction.Username);
                    }
                    responseString = res ? "Player Already Spawned." :
                                        "Player wasnt already spawned. Added Player to spawn queue.";
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    responseString = e.ToString();
                }

            }
            else
            {
                responseString = "Error: Interpolator not setup";
            }

            
            await Clients.Caller.ReceiveString(responseString);
        }

    }
}
