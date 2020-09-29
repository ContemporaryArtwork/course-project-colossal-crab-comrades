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
        private readonly LoginService _ls;

        public LoginController(LoginService ls)
        {
            _ls = ls;
        }



        [HttpPost]
        
        public void post(string username, string password)
        {

            try
            {
                Response.Cookies.Append("auth-token", _ls.SignIn(username, password));

            }
            catch(UserDoesNotExistException e){
                //Change for later
                _ls.SignUp(username, password);
            }
            
        }
        


       


    }
}
