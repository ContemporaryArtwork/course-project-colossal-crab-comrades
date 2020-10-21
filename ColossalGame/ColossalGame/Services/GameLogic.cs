using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        private Dictionary<string,PlayerModel> PlayerDictionary { get; set; } = new Dictionary<string, PlayerModel>();
        private List<GameObjectModel> ObjectList {get;set;} = new List<GameObjectModel>();
        private readonly LoginService _ls;
        private readonly UserService _us;
        private bool KeepGoing = true;
        private int tickRate = 50;
        private int tickCounter = 0;
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public GameLogic(LoginService ls, UserService us)
        {
            _ls = ls;
            _us = us;
        }
        private void HandleAction(AUserAction a)
        {
            
            if (a is MovementAction m)
            {
                PlayerModel pm = PlayerDictionary.GetValueOrDefault(m.Username);
                if (pm==null) throw new Exception("Player must be spawned first!");
                if (m.Direction == EDirection.Down)
                {
                    pm.YPos -= 1;
                }
                else if (m.Direction == EDirection.Up)
                {
                    pm.YPos += 1;
                }
                else if (m.Direction == EDirection.Left)
                {
                    pm.XPos -= 1;
                }
                else if (m.Direction == EDirection.Right)
                {
                    pm.XPos += 1;
                }

                PlayerDictionary.Add(m.Username,pm);
            }
        }

        public void SpawnPlayer(string username, double xPos = 0.0, double yPos = 0.0)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();

            PlayerModel pm = new PlayerModel();
            pm.Username = username;
            pm.XPos = xPos;
            pm.YPos = yPos;

            PlayerDictionary.Add(username,pm);
        }

        public void DespawnPlayer(string username)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();
            PlayerDictionary.Remove(username);
        }

        public void simulateOneServerTick()
        {
            //TODO
        }

        public (List<GameObjectModel>,Dictionary<string,PlayerModel>) getState()
        {
            //TODO
            return (null,null);
        }

        public async void Start()
        {
            await Task.Run(runServer);
        }
        private async void runServer()
        {
            while (KeepGoing)
            {
                System.Threading.Thread.Sleep(tickRate);
                simulateOneServerTick();
                tickCounter++;
                //TODO: Add logic dictating when to publish the state instead of after every tick
                PublishState();
            }
        }

        private void PublishState()
        {
            var (returnedList, returnedDictionary) = getState();
            CustomEventArgs e = new CustomEventArgs(returnedList,returnedDictionary);
            OnRaiseCustomEvent(e);
        }

        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            RaiseCustomEvent?.Invoke(this, e);
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(List<GameObjectModel> objectList,Dictionary<string,PlayerModel> playerDict)
        {
            ObjectList = objectList;
            PlayerDict = playerDict;
        }

        public List<GameObjectModel> ObjectList { get; set; }

        public Dictionary<string, PlayerModel> PlayerDict { get; set; }

    }
}
