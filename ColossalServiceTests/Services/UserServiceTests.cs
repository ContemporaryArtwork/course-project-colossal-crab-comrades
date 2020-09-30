using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using ColossalGame.Models;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {


        [SetUp]
        public void SetUp()
        {

        }

        private UserService CreateService()
        {
            return new UserService();
        }

        [Test]
        public void Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.Get();

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Get_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var service = this.CreateService();
            string id = null;

            // Act
            var result = service.Get(
                id);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            User user = null;

            // Act
            var result = service.Create(
                user);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Update_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string id = null;
            User userIn = null;

            // Act
            service.Update(
                id,
                userIn);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Remove_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            User userIn = null;

            // Act
            service.Remove(
                userIn);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Remove_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var service = this.CreateService();
            string id = null;

            // Act
            service.Remove(
                id);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void GetByUsername_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string username = null;

            // Act
            var result = service.GetByUsername(
                username);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void UserExistsByUsername_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string username = null;

            // Act
            var result = service.UserExistsByUsername(
                username);

            // Assert
            Assert.Fail();
        }

        [Test]
        public void generateToken_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            User userIn = null;

            // Act
            var result = service.generateToken(
                userIn);

            // Assert
            Assert.Fail();
        }
    }
}
