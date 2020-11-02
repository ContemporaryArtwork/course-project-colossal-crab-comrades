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
            _subLoginService.DeleteUser("realUser2");
            _subLoginService.SignUp("realUser", "123");
            _subLoginService.SignUp("realUser2", "123");

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
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
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

            Assert.True(a.IsNullOrEmpty() && b.IsNullOrEmpty());
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
            Assert.True(a.IsNullOrEmpty() && b.IsNullOrEmpty());
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
            Assert.True(a.IsNullOrEmpty() && b.IsNullOrEmpty());
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

        [Test]

        public void AddActionToQueue_ValidActionRightAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 0.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(0.0, b.ElementAt(0).Value.YPos);

        }

        [Test]

        public void AddActionToQueue_ValidActionUpAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 0.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Up;
            ma.Token = null;
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
            Assert.AreEqual(1.0, b.ElementAt(0).Value.YPos);

        }

        [Test]

        public void AddActionToQueue_ValidActionDownAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 1.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Down;
            ma.Token = null;
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
            Assert.AreEqual(0.0, b.ElementAt(0).Value.YPos);

        }

        [Test]

        public void AddActionToQueue_TwoValidActionUpRightAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 0.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Up;
            ma.Token = null;
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);

            MovementAction ma2 = new MovementAction();
            ma2.Direction = EDirection.Right;
            ma2.Token = null;
            ma2.Username = username;
            gameLogic.AddActionToQueue(ma2);

            // Assert
            Thread.Sleep(200);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.YPos);

        }

        [Test]

        public void AddActionToQueue_ValidActionDownToNegativeAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 0.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Down;
            ma.Token = null;
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
            Assert.AreEqual(-1.0, b.ElementAt(0).Value.YPos);

        }

        [Test]

        public void AddActionToQueue_ValidAction_DespawnPlayerOnNextServerTick()
        {
            // Do an action and then despawn the player

            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 0.0;
            double yPos = 1.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Down;
            ma.Token = null;
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
            Assert.AreEqual(0.0, b.ElementAt(0).Value.YPos);

            gameLogic.AddPlayerToDespawnQueue("realUser");
            Thread.Sleep(100);
            (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty() && b.IsNullOrEmpty());
        }

        [Test]

        public void SpawnTwoPlayers_DifferentServerTick()
        {
            // See if game can suppost spawning in two players in different ticks

            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            Thread.Sleep(100);
       
            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check P1
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);

            // Check P2
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(5.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(1).Value.YPos);
        }

        [Test]

        public void SpawnTwoPlayers_SameServerTick()
        {
            // See if the game can support spawning in two players at the same time
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check P1
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);

            // Check P2
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(5.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(1).Value.YPos);
        }

        [Test]

        public void SpawnTwoPlayers_DespawnMostRecentlySpawned()
        {
            // Spawn in two people and try despawning the one of them
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            Thread.Sleep(100);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);

            gameLogic.AddPlayerToDespawnQueue("realUser2");

            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if only P1 remains
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);

            
        }

        [Test]

        public void SpawnTwoPlayers_DespawnFirstPlayerSpawned()
        {
            // Spawn in two people and try despawning the one of them
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            Thread.Sleep(100);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);

            gameLogic.AddPlayerToDespawnQueue("realUser");

            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if only P2 remains
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser2", b.ElementAt(0).Key);
            Assert.AreEqual("realUser2", b.ElementAt(0).Value.Username);
            Assert.AreEqual(5.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(0).Value.YPos);


        }

        [Test]
        
        public void SpawnTwoPlayers_ValidMovementActionFor1()
        {
            // Move the first player and not the second
            // Arrange 1
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
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if P1 moved
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
            // Check if P2 is the same
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(5.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(1).Value.YPos);

        }

        [Test]

        public void SpawnTwoPlayers_ValidMovementActionFor2()
        {
            // Move the second player and not the first
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            double xPos = 1.0;
            double yPos = 2.0;

            // Act
            gameLogic.AddPlayerToSpawnQueue(
                username,
                xPos,
                yPos);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username2;
            gameLogic.AddActionToQueue(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if P1 moved
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(1.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
            // Check if P2 is the same
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(6.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(1).Value.YPos);

        }

        [Test]

        public void SpawnTwoPlayers_ValidMovementActionForBothPlayers()
        {
            // Move both players in different directions
            // Arrange 1
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
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 5.0;
            double yPos2 = 4.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            MovementAction ma2 = new MovementAction();
            ma2.Direction = EDirection.Right;
            ma2.Token = null;
            ma2.Username = username2;
            gameLogic.AddActionToQueue(ma2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if P1 moved
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
            // Check if P2 is the same
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(6.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(4.0, b.ElementAt(1).Value.YPos);

        }

        [Test]
        public void TwoPlayers_ValidMovmentActionPlayer_PositionTheSame()
        {
            // Spawn two player and have them move to the same spot
            // Arrange 1
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
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.AddActionToQueue(ma);

            // Arrange 2
            string username2 = "realUser2";
            double xPos2 = 3.0;
            double yPos2 = 2.0;

            // Act 2
            gameLogic.AddPlayerToSpawnQueue(
                username2,
                xPos2,
                yPos2);

            MovementAction ma2 = new MovementAction();
            ma2.Direction = EDirection.Left;
            ma2.Token = null;
            ma2.Username = username2;
            gameLogic.AddActionToQueue(ma2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if P1 moved
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual("realUser", b.ElementAt(0).Value.Username);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(0).Value.YPos);
            // Check if P2 is the same
            Assert.AreEqual("realUser2", b.ElementAt(1).Key);
            Assert.AreEqual("realUser2", b.ElementAt(1).Value.Username);
            Assert.AreEqual(2.0, b.ElementAt(1).Value.XPos);
            Assert.AreEqual(2.0, b.ElementAt(1).Value.YPos);


        }

    }
}

    
