using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class LoginServiceTests
    {
        private UserService subUserService;

        [SetUp]
        public void SetUp()
        {
            this.subUserService = Substitute.For<UserService>("mongodb://localhost:27017/");
        }

        private LoginService CreateService()
        {
            return new LoginService(
                this.subUserService);
        }

        private (string username, string password) GenerateValidLogin()
        {
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = "123456";
            return (username, password);
        }

        [Test]
        public void SignIn_ValidUsernameValidPassword_ReturnHashedToken()
        {
            // Arrange
            var service = this.CreateService();
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = "test";
            service.DeleteUser(username);
            service.SignUp(username, password);

            // Act
            var result = service.SignIn(
                username,
                password);


            Assert.NotNull(result);
            service.DeleteUser(username);
            
        }

        [Test]
        public void SignIn_ValidUsernameInvalidPassword_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = "test";
            service.DeleteUser(username);
            service.SignUp(username, password);

            // Act
            bool correctException = false;
            try
            {
                var result = service.SignIn(
                    username,
                    "badpassword");
            }
            catch (IncorrectPasswordException)
            {
                correctException = true;
            }
            


            Assert.True(correctException);
            service.DeleteUser(username);

        }

        [Test]
        public void SignIn_ValidUsernameNullPassword_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = "test";
            service.DeleteUser(username);
            service.SignUp(username, password);

            // Act
            bool correctException = false;
            try
            {
                var result = service.SignIn(
                    username,
                    null);
            }
            catch (IncorrectPasswordException)
            {
                correctException = true;
            }



            Assert.True(correctException);
            service.DeleteUser(username);

        }

        [Test]
        public void SignIn_NullUsername_ThrowException()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            bool correctException = false;
            try
            {
                var result = service.SignIn(
                    null,
                    "badpassword");
            }
            catch (BadUsernameException)
            {
                correctException = true;
            }

            Assert.True(correctException);
            
        }

        [Test]
        public void SignUp_ValidUsernameValidPassword_ReturnTrue()
        {
            // Arrange
            var service = this.CreateService();
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = "test";
            service.DeleteUser(username);


            // Act
            var result = service.SignUp(username, password);

            Assert.True(result);
            service.DeleteUser(username);
        }

        [Test]
        public void SignUp_NullUsername_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            string username = null;
            string password = "test";

            // Act
            bool throws = false;
            try
            {
                var result = service.SignUp(username, password);
            }
            catch (BadUsernameException)
            {
                throws = true;
            }

            Assert.True(throws);
        }

        [Test]
        public void SignUp_ValidUsernameNullPassword_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string username = Convert.ToBase64String(bytes);
            string password = null;

            
            Assert.False(subUserService.UserExistsByUsername(username));
            


            // Act
            bool throws = false;
            try
            {
                var result = service.SignUp(username, password);
            }
            catch (BadPasswordException)
            {
                throws = true;
            }

            Assert.True(throws);
        }

        [Test]
        public void VerifyToken_ValidTokenValidUsername_ReturnTrue()
        {
            // Arrange
            var service = this.CreateService();
            var (username, password) = GenerateValidLogin();
            service.DeleteUser(username);
            Assert.False(subUserService.UserExistsByUsername(username));
            Assert.True(service.SignUp(username,password));
            string token = service.SignIn(username,password);
            Assert.NotNull(token);
            

            // Act
            var result = service.VerifyToken(
                token,
                username);

            // Assert
            Assert.True(result);
            service.DeleteUser(username);
        }

        [Test]
        public void VerifyToken_InvalidTokenValidUsername_ReturnFalse()
        {
            // Arrange
            var service = this.CreateService();
            var (username, password) = GenerateValidLogin();
            service.DeleteUser(username);
            Assert.False(subUserService.UserExistsByUsername(username));
            Assert.True(service.SignUp(username, password));
            string token = service.SignIn(username, password);
            Assert.NotNull(token);


            // Act
            var result = service.VerifyToken(
                "badtoken",
                username);

            // Assert
            Assert.False(result);
            service.DeleteUser(username);
        }
        [Test]
        public void VerifyToken_InvalidUsername_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            var (username, password) = GenerateValidLogin();
            service.DeleteUser(username);
            Assert.False(subUserService.UserExistsByUsername(username));
            


            // Act
            var throws = false;
            try
            {
                var result = service.VerifyToken(
                    "badtoken",
                    username);
            }
            catch (UserDoesNotExistException)
            {
                throws = true;
            }

            // Assert
            Assert.True(throws);
            
        }

        [Test]
        public void VerifyToken_NullUsername_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            var (username, password) = GenerateValidLogin();
            service.DeleteUser(username);
            Assert.False(subUserService.UserExistsByUsername(username));



            // Act
            var throws = false;
            try
            {
                var result = service.VerifyToken(
                    "badtoken",
                    username);
            }
            catch (UserDoesNotExistException)
            {
                throws = true;
            }

            // Assert
            Assert.True(throws);

        }
        [Test]
        public void VerifyToken_ValidUsernameNullToken_ThrowException()
        {
            // Arrange
            var service = this.CreateService();
            var (username, password) = GenerateValidLogin();
            service.DeleteUser(username);
            Assert.False(subUserService.UserExistsByUsername(username));



            // Act
            var throws = false;
            try
            {
                var result = service.VerifyToken(
                    "badtoken",
                    username);
            }
            catch (UserDoesNotExistException)
            {
                throws = true;
            }

            // Assert
            Assert.True(throws);

        }

        [Test]
        public void DeleteUser_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string username = null;
            string password = null;

            // Act
            var result = service.DeleteUser(
                username);

            // Assert
            //Assert.Fail();
        }
    }
}
