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
using System.Xml.Serialization;


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

        [HttpPost("/api/login")]
        public string[] login([FromForm] Models.FormModel user)
        {
            //_ls = new LoginService(new UserService());
            try
            {

                string username = user.Username;
                string password = user.Password;
                
                var response = _ls.SignIn(username, password);
                Response.Cookies.Append("auth-token", res);

                string[] output = new string[]
                    {
                        "You are now signed in as ",
                        username,
                        "your authentication token is",
                        response
                    };
                return output;
            }
            catch(UserDoesNotExistException e){

                string[] output = new string[]
                    {
                        "User does not exists"
                    };
                return output;
            }
            catch(IncorrectPasswordException e)
            {
                string[] output = new string[]
                   {
                        "Incorrect Password"
                   };
                return output;
            }
           
            
        }
        [HttpPost("/api/signup")]
        public string[] signup([FromForm] Models.FormModel user)
        {
            //_ls = new LoginService(new UserService());
            try
            {
                string username = user.Username;
                string password = user.Password;

                _ls.SignUp(username, password);

                string[] output = new string[]
                  {
                        "You are now signed in as ",
                        username,
                        " with the password ",
                        password
                  };
                
                return output;
            }
            catch (UserAlreadyExistsException e)
            {
                string[] output = new string[]
              {
                    "User already exists"
              };
                return output;
            }
            catch (BadPasswordException e)
            {
                string[] output = new string[]
              {
                    "Bad password"
              };
                return output;
            }
            catch (BadUsernameException e)
            {
                string[] output = new string[]
              {
                    "Bad username"
              };
                return output;
            }


        }






    }
}
