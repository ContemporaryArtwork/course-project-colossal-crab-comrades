using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using ColossalGame.Models;
using System.Threading.Tasks;
using System.Threading;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class InterpolatorTests
    {
        private LoginService subLoginService;
        private UserService subUserService;
        private GameLogic subGameLogic;

        [SetUp]
        public void SetUp()
        {
            this.subUserService = Substitute.For<UserService>("mongodb://localhost:27017/");
            this.subLoginService = Substitute.For<LoginService>(this.subUserService);
            this.subGameLogic = Substitute.For<GameLogic>(this.subLoginService,this.subUserService);
            this.subLoginService.DeleteUser("testUser");
            this.subLoginService.SignUp("testUser", "Testpassword123$");
            this.subGameLogic.HandleSpawnPlayer("testUser");
        }

        private Interpolator CreateInterpolator()
        {
            return new Interpolator(
                this.subLoginService, this.subGameLogic);
        }

        [Test]
        public void ParseAction_ValidLogin_ReturnTrue()
        {
            // Arrange
            var interpolator = this.CreateInterpolator();
            MovementAction action = new MovementAction();

            //this.subLoginService.DeleteUser("testUser");
            //this.subLoginService.SignUp("testUser", "Testpassword123$");
            
            string token = this.subLoginService.SignIn("testUser", "Testpassword123$");

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

            //subLoginService.DeleteUser("testUser");
            //subLoginService.SignUp("testUser", "Testpassword123$");
            string token = subLoginService.SignIn("testUser", "Testpassword123$");


            action.Username = "testUser";
            action.Token = token;

            // Act
            Assert.True( interpolator.ParseAction(action));
            Thread.Sleep(5);
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

            //subLoginService.DeleteUser("testUser");
            //subLoginService.SignUp("testUser", "Password123$");
            string token = subLoginService.SignIn("testUser", "Testpassword123$");


            action.Username = "testUser";
            action.Token = token;

            // Act
            Assert.True(interpolator.ParseAction(action));
            Thread.Sleep(55);
            bool result = interpolator.ParseAction(action);

            //Assert
            Assert.True(result);

        }
        
    }
}
