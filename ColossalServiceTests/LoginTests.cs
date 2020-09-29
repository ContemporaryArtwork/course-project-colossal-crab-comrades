using System;
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
            Console.WriteLine(_loginService.DeleteUser("coolAccount","coolpassword"));
            bool createdSuccessfully = _loginService.SignUp("coolAccount", "coolpassword");
            Assert.True(createdSuccessfully);
            Assert.Throws<UserAlreadyExistsException>((() => _loginService.SignUp("coolAccount","coolpassword")));
        }
    }
}