using ColossalGame.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using ColossalGame.Models;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class GameLogicTests
    {
        private LoginService _subLoginService;
        private UserService _subUserService;

        [SetUp]
        public void SetUp()
        {
            this._subUserService = Substitute.For<UserService>("mongodb://localhost:27017/");
            this._subLoginService = Substitute.For<LoginService>(this._subUserService);
            _subLoginService.DeleteUser("realUser");
            _subLoginService.SignUp("realUser", "123");

        }

        private GameLogic CreateGameLogic()
        {
            return new GameLogic(
                this._subLoginService,
                this._subUserService);
        }

        [Test]
        public void SpawnPlayer_ValidPlayer_DictionaryIncludesPlayer()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();
            
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1,b.Count);
            Assert.AreEqual("realUser",b.ElementAt(0).Key);
            Assert.AreEqual("realUser",b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0,b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
        }

        [Test]
        public void SpawnPlayer_ServerStopped_ExpectEmptyState()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            gameLogic.StopServer();
            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            // Assert

            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty()&&b.IsNullOrEmpty());
        }

        [Test]
        public void DespawnPlayer_PlayerExists_PlayerLeavesDictionary()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.True(b.Count == 1 && b.ElementAt(0).Key == "realUser" && b.ElementAt(0).Value.Username == "realUser" && b.ElementAt(0).Value.XPos == 1.0 && b.ElementAt(0).Value.YPos == 2.0);
            gameLogic.AddPlayerToDespawnQueue("realUser");
            Thread.Sleep(100);
            (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty()&&b.IsNullOrEmpty());
        }
        
        [Test]
        public void DespawnPlayer_UsernameNull_ErrorThrown()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            
            var (a, b) = gameLogic.GetState();

            Assert.Throws<UserDoesNotExistException>(delegate
            {
                gameLogic.AddPlayerToDespawnQueue(null);
            });
            Thread.Sleep(100);
            (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty()&&b.IsNullOrEmpty());
        }

        [Test]
        public void AddActionToQueue_ValidActionLeftAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Left;
            ma.Token = null;//only relevant for interpolator
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);
            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(0.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
        }
    }
}
