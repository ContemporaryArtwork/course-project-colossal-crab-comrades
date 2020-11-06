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

        public enum ErrorTypes
        {
            UserAlreadyExists,
            BadPassword,
            BadUsername,
            Unknown
        }
        public LoginController(LoginService ls)
        {
            _ls = new LoginService(new UserService());
        }


        [HttpPost("/api/login")]
        public string login([FromForm] Models.FormModel user)
        {
            var output = new Dictionary<string, string>();
            try
            {
                //Define credentials
                var username = user.Username;
                var password = user.Password;
                //Try login service method
                var token = _ls.SignIn(username, password);
                //If successful
                var usernameCookieOptions = new CookieOptions {SameSite = SameSiteMode.Strict};
                var authTokenCookieOptions = new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict};
                
                Response.Cookies.Append("username", username,usernameCookieOptions);
                Response.Cookies.Append("auth-token", token,authTokenCookieOptions);
                output["status"] = "ok";
                output["message"] = "Sign in successful";
            }
            catch(UserDoesNotExistException){
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadUsername.ToString("G");
                output["message"] = "Username does not exist";
            }
            catch(IncorrectPasswordException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadPassword.ToString("G");
                output["message"] = "Incorrect or otherwise bad password";
            }
            catch (Exception e)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.Unknown.ToString("G");
                output["message"] = "Unknown Error Encountered of type: " + e.GetType();
            }

            return JsonConvert.SerializeObject(output, Formatting.Indented);
        }

        [HttpPost("/api/signup")]
        public string signup([FromForm] Models.FormModel user)
        {
            var output = new Dictionary<string, string>();
            
            try
            {
                var username = user.Username;
                var password = user.Password;
                _ls.SignUp(username, password);

                output["status"] = "ok";
                output["message"] = "Sign up successful";
            }
            catch (UserAlreadyExistsException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.UserAlreadyExists.ToString("G");
                output["message"] = "Username already exists";
            }
            catch (BadPasswordException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadPassword.ToString("G");
                output["message"] = "Password is formatted poorly. Password requirements: 8 characters, 1 uppercase, 1 lowercase, 1 special character.";
            }
            catch (BadUsernameException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadUsername.ToString("G");
                output["message"] = "Username is formatted poorly";
            }
            catch (Exception e)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.Unknown.ToString("G");
                output["message"] = "Unknown Error Encountered of type: " + e.GetType();
            }

            return JsonConvert.SerializeObject(output, Formatting.Indented);
        }






    }
}
