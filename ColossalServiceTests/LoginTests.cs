using System;
using ColossalGame.Controllers;
using ColossalGame.Services;
using NUnit.Framework;

namespace ColossalServiceTests
{
    [TestFixture]
    public class LoginTests
    {
        private LoginService _loginService;
        [SetUp]
        public void Setup()
        {
            _loginService = new LoginService(new UserService());
        }

        [Test]
        public void TestSignUp()
        {
            Console.WriteLine(_loginService.DeleteUser("coolAccount"));
            bool createdSuccessfully = _loginService.SignUp("coolAccount", "coolpassword");
            Assert.True(createdSuccessfully);
            Assert.Throws<UserAlreadyExistsException>((() => _loginService.SignUp("coolAccount","coolpassword")));
        }

        [Test]
        public void TestSignIn()
        {
            Console.WriteLine(_loginService.DeleteUser("coolAccount"));
            Assert.True( _loginService.SignUp("coolAccount", "coolpassword"));
            Assert.NotNull( _loginService.SignIn("coolAccount", "coolpassword"));
        }

        [Test]
        public void TestSignInUserDNE()
        {
            Guid g = new Guid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            Assert.Throws<UserDoesNotExistException>((() => _loginService.SignIn(guidString, "1234")));
        }

        [Test]
        public void TestSignInUserNull()
        {
            Assert.Throws<UserDoesNotExistException>((() => _loginService.SignIn(null, "1234")));
        }

        [Test]
        public void TestSignInPasswordNull()
        {
            Assert.Throws<UserDoesNotExistException>((() => _loginService.SignIn("null", null)));
        }
    }
}