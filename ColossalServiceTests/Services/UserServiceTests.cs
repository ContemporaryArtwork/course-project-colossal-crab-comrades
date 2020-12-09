using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using ColossalGame.Models;
using MongoDB.Bson;

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
            return new UserService("mongodb://127.0.0.1:27017");
        }
        
        [Test]
        public void Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act


            var result = service.Get();

            // Only checking if the get function doesnt cause an error
            // Assert
            Assert.Pass();
        }

        [Test]
        public void Get_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();


            // Act
            var result = service.Get(
                id.ToString());
            // Only checking if the get function doesnt cause an error
            // Assert
            Assert.Pass();
        }
        
        [Test]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            User user = new User();

            // Act
            var result = service.Create(
                user);

            // Assert
            Assert.AreEqual(user, result);

        }

        [Test]
        public void Update_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();
            User user = new User();
            user.Username = id.ToString();
            user.Id = id.ToString();

            service.Create(user);


            BsonObjectId id2 = ObjectId.GenerateNewId();
            
            
            var result1 = service.UserExistsByUsername(id2.ToString());
            user.Username = id2.ToString();
            // Act
            service.Update(
                user.Id,
                user);

            var result2 = service.UserExistsByUsername(id2.ToString());
            // Assert
            Assert.IsFalse(result1);
            Assert.IsTrue(result2);

        }

        [Test]
        public void RemoveGivenUser_StateUnderTest_ExpectedBehavior()
        {

            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();

            User user = new User();
            user.Username = id.ToString();
            user.Id = id.ToString();


            service.Create(user);

            var existsBeforeRemove = service.UserExistsByUsername(id.ToString());

            // Act
            service.Remove(user);

            var existsAfterRemove = service.UserExistsByUsername(id.ToString());

            // Assert
            Assert.IsTrue(existsBeforeRemove);
            Assert.IsFalse(existsAfterRemove);


        }

        [Test]
        public void RemoveGivenId_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();
            
            User user = new User();
            user.Username = id.ToString();
            user.Id = id.ToString();


            service.Create(user);

            var existsBeforeRemove = service.UserExistsByUsername(id.ToString());
            
            // Act
            service.Remove(id.ToString());

            var existsAfterRemove = service.UserExistsByUsername(id.ToString());

            // Assert
            Assert.IsTrue(existsBeforeRemove);
            Assert.IsFalse(existsAfterRemove);
            
        }

        [Test]
        public void GetByUsername_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();
            string username = id.ToString();


            User user = new User();
            user.Username = username;

            service.Create(user);
            // Act
            var result = service.GetByUsername(username);
            var result2 = service.GetByUsername("notaUser");
            // Assert
            Assert.AreEqual(username, result.Username);
            Assert.IsNull(result2);
        }

        [Test]
        public void UserExistsByUsername_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            BsonObjectId id = ObjectId.GenerateNewId();
            string username = id.ToString();

            User user = new User();
            user.Username = username;
            service.Create(user);

            // Act
            var result = service.UserExistsByUsername(
                username);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void generateToken_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            User user = new User();

            // Act
            var token1 = service.generateToken(
                user);

            var hash1 = user.TokenHash;
            var time1 = user.TokenAge;

            var token2 = service.generateToken(
                user);

            var hash2 = user.TokenHash;
            var time2 = user.TokenAge;

            // Assert
            Assert.IsFalse(String.Equals(hash1, hash2));
            Assert.AreNotEqual(time1, time2);
            Assert.IsTrue((DateTime.Now - user.TokenAge) < TimeSpan.FromSeconds(.1d));
            // Ensures random token generation 
            Assert.IsFalse(String.Equals(token1, token2));

            
           
        }
    }
}
