using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ColossalGame.Models;
using ColossalGame.Models.Exceptions;
using ColossalGame.Models.GameModels;

namespace ColossalGame.Services
{
    public class Interpolator : IInterpolator
    {

        public double MovementInterval { get; set; } = 17;
        public double ShootingInterval { get; set; } = 200;
        private readonly ConcurrentDictionary<string, DateTime> _movementTimings = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, DateTime> _shootingTimings = new ConcurrentDictionary<string, DateTime>();

        private readonly LoginService _ls;
        private readonly GameLogic _gl;

        public Interpolator(LoginService ls, GameLogic gl)
        {
            _ls = ls;
            _gl = gl;
        }

        /// <summary>
        /// Method to parse actions from the client on a timer. First, it will check if the user passed in is currently authenticated. If it isn't, then an error will be thrown.
        /// Next, it will check if the user has made an action in the last {default=50} milliseconds. If they have, return false. Otherwise, it updates the state of the game and then resets the player's
        /// last action time in memory.
        /// </summary>
        /// <param name="action">A JSON object representing the action</param>
        /// <returns>A boolean representing whether the action was done successfully</returns>
        public bool ParseAction(AUserAction action)
        {

            //Make sure that the user is authenticated
            if (!_ls.VerifyToken(action.Token, action.Username))
            {
                throw new InvalidLoginException("Either the token or the username is invalid.");
            }

            try
            {
                if (action is MovementAction)
                {

                    //If the action's user DNE
                    if (!_movementTimings.ContainsKey(action.Username))
                    {

                        _movementTimings.TryAdd(action.Username, DateTime.Now);
                        //Run relevant method to update game state
                        _gl.HandleAction(action);
                        //
                        return true;
                    }

                    //fetch last update time
                    DateTime x = _movementTimings[action.Username];

                    var ts = DateTime.Now - x;

                    //It's been at least {MovementInterval} milliseconds since last action
                    if (ts.TotalMilliseconds >= MovementInterval)
                    {
                        _movementTimings[action.Username] = DateTime.Now;
                        //Run relevant method to update game state
                        _gl.HandleAction(action);
                        //
                        return true;
                    }

                    return false;
                }
                else if (action is ShootingAction)
                {


                    //If the action's user DNE
                    if (!_shootingTimings.ContainsKey(action.Username))
                    {

                        _shootingTimings.TryAdd(action.Username, DateTime.Now);
                        //Run relevant method to update game state
                        _gl.HandleAction(action);
                        //
                        return true;
                    }

                    //fetch last update time
                    DateTime x = _shootingTimings[action.Username];

                    var ts = DateTime.Now - x;

                    //It's been at least {MovementInterval} milliseconds since last action
                    if (ts.TotalMilliseconds >= ShootingInterval)
                    {
                        _shootingTimings[action.Username] = DateTime.Now;
                        //Run relevant method to update game state
                        _gl.HandleAction(action);
                        //
                        return true;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (UnspawnedException e)
            {
                return false;
            }
        }


    }

    public interface IInterpolator
    {
        public bool ParseAction(AUserAction action);
    }

    [Serializable]
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException()
        {
        }

        public InvalidLoginException(string message) : base(message)
        {
        }

        public InvalidLoginException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
