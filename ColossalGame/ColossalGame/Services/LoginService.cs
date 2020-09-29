using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models;

namespace ColossalGame.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserService _us;
        public LoginService(UserService us)
        {
            _us = us;
        }

        public string SignIn(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool SignUp(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool VerifyToken(string token)
        {
            throw new NotImplementedException();
        }
    }

    public interface ILoginService
    {
        public bool SignUp(string username, string password);
        public string SignIn(string username, string password);
        public bool VerifyToken(string token);
    }
}
