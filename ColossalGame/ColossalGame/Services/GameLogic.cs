using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models;

namespace ColossalGame.Services
{
    public class GameLogic
    {
        private Dictionary<string,PlayerModel> PlayerDictionary = new Dictionary<string, PlayerModel>();
        private readonly LoginService _ls;
        private readonly UserService _us;
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

        public List<GameObjectModel> getState()
        {
            //TODO
            return null;
        }
    }
}
