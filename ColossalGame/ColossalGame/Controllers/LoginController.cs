using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Net.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ColossalGame.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase
    {
        private LoginService _ls;

        public LoginController(LoginService ls)
        {
            _ls = new LoginService(new UserService());
        }

        [HttpGet]
        public string Test()
        {
            return "test";
        }

        [HttpPost]
        public void post(string username, string password)
        {
            //_ls = new LoginService(new UserService());
            try
            {
                //_ls.SignIn(username, password);
                var res = _ls.SignIn(username, password);
                Response.Cookies.Append("auth-token", res);

            }
            catch(UserDoesNotExistException e){
                //Change for later
                _ls.SignUp(username, password);
            }
            
        }
        


       


    }
}
