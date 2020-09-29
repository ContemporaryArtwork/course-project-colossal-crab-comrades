using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ColossalGame.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase
    {
        private LoginService ls;

        public LoginController()
        {
            ls = new LoginService();
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "bruh" };
        }


        [HttpPost]
        public string Post(string username, string password)
        {
            //verify username and password

            return "post is working";
        }


    }
}
