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
            MalformedToken,
            BadToken,
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
                Response.Cookies.Append("auth-token", token, usernameCookieOptions); //**Note** As of now the frontend needs the authtoken to not be HttpOnly. If we have the time available research ways we can use this cookie in frontend while set to httpOnly=true.
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

        [HttpGet("/api/loggedIn")]
        public string loggedIn()
        {
            var output = new Dictionary<string, string>();
            try
            {
                string username;
                string token;
                if (Request.Cookies["username"] != null && Request.Cookies["auth-token"] != null)
                {
                    username = Request.Cookies["username"];
                    token = Request.Cookies["auth-token"];

                    //Try login service method
                    var loggedIn = _ls.VerifyToken(token, username);
                    output["status"] = "ok";
                    output["message"] = "You are logged in";
                }
                else
                {
                    output["status"] = "error";
                    output["message"] = "You need to login first";
                }
                
                
            }
            catch (BadTokenException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.MalformedToken.ToString("G");
                output["message"] = "Token Malformed";
            }
            catch (BadUsernameException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadUsername.ToString("G");
                output["message"] = "Incorrect or otherwise bad username";
            }
            catch (UserDoesNotExistException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadUsername.ToString("G");
                output["message"] = "Incorrect or otherwise bad username";
            }
            catch (TokenExpiredException)
            {
                output["status"] = "error";
                output["errorCode"] = ErrorTypes.BadToken.ToString("G");
                output["message"] = "Your token is out of date";
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
