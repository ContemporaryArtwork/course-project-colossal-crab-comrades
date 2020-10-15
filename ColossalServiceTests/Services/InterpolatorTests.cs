using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using ColossalGame.Models;
using System.Threading.Tasks;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class InterpolatorTests
    {
        private LoginService subLoginService;
        private UserService subUserService;

        [SetUp]
        public void SetUp()
        {
            this.subUserService = Substitute.For<UserService>("mongodb://localhost:27017/");
            this.subLoginService = Substitute.For<LoginService>(subUserService);
        }

        private Interpolator CreateInterpolator()
        {
            return new Interpolator(
                this.subLoginService);
        }

        [Test]
        public void ParseAction_ValidLogin_ReturnTrue()
        {
            // Arrange
            var interpolator = this.CreateInterpolator();
            MovementAction action = new MovementAction();

            subLoginService.DeleteUser("testUser");
            subLoginService.SignUp("testUser", "testPassword");
            string token = subLoginService.SignIn("testUser", "testPassword");


            action.Username = "testUser";
            action.Token = token;

            // Act
            bool result = interpolator.ParseAction(action);


            //Assert
            Assert.True(result);

        }

        [Test]
        public void ParseAction_ValidLoginButSecondActionTooFast_ReturnFalse()
        {
            // Arrange
            var interpolator = this.CreateInterpolator();
            MovementAction action = new MovementAction();

            subLoginService.DeleteUser("testUser");
            subLoginService.SignUp("testUser", "testPassword");
            string token = subLoginService.SignIn("testUser", "testPassword");


            action.Username = "testUser";
            action.Token = token;

            // Act
            Assert.True( interpolator.ParseAction(action));
            System.Threading.Thread.Sleep(20);
            bool result = interpolator.ParseAction(action);

            //Assert
            Assert.False(result);

        }

        [Test]
        public void ParseAction_ValidLoginButSecondActionJustABitSlow_ReturnTrue()
        {
            // Arrange
            var interpolator = this.CreateInterpolator();
            MovementAction action = new MovementAction();

            subLoginService.DeleteUser("testUser");
            subLoginService.SignUp("testUser", "testPassword");
            string token = subLoginService.SignIn("testUser", "testPassword");


            action.Username = "testUser";
            action.Token = token;

            // Act
            Assert.True(interpolator.ParseAction(action));
            System.Threading.Thread.Sleep(55);
            bool result = interpolator.ParseAction(action);

            //Assert
            Assert.True(result);

        }
    }
}
