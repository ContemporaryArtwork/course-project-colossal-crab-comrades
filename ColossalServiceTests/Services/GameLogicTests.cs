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
using tainicom.Aether.Physics2D.Common;

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
            _subLoginService.SignUp("realUser", "Password123$");
            _subLoginService.SignUp("realUser2", "Password123$");

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

            // Act
            gameLogic.HandleSpawnPlayer(username);
                
            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();
            
            Assert.True(a.IsNullOrEmpty());
            Assert.True(b.ContainsKey("realUser"));
            Assert.True(gameLogic.IsPlayerSpawned("realUser"));
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }   

        
        
        [Test]
        public void SpawnPlayer_ServerStopped_ExpectEmptyState()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            gameLogic.StopServer();
            Thread.Sleep(1000);
            // Act
            gameLogic.HandleSpawnPlayer(username);
                

            // Assert

            var (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty());
            
        }

        /*
        [Test]
        public void DespawnPlayer_PlayerExists_PlayerLeavesDictionary()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            float xPos = 1.0f;
            float yPos = 2.0f;

            // Act
            gameLogic.HandleSpawnPlayer(
                username,
                xPos,
                yPos);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();
            
            Assert.True(a.IsNullOrEmpty());
            Assert.True(b.Count == 1);
            Assert.True(b.ElementAt(0).Key == "realUser");
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            gameLogic.DespawnPlayer("realUser");
            Thread.Sleep(100);
            (a, b) = gameLogic.GetState();
            Assert.True(b.Count == 0);
            Assert.False(b.ContainsKey("realUser"));
        }
        
        
        [Test]
        public void DespawnPlayer_UsernameNull_ErrorThrown()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();

            var (a, b) = gameLogic.GetState();

            Assert.Throws<UserDoesNotExistException>(delegate
            {
                gameLogic.DespawnPlayer(null);
            });
            Thread.Sleep(100);
            (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty() && b.IsNullOrEmpty());
        }
        */
        
        [Test]
        public void AddActionToQueue_ValidActionLeftAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";


            // Act
            gameLogic.HandleSpawnPlayer(username);
                

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Left;
            ma.Token = null;//only relevant for interpolator
            ma.Username = username;
            gameLogic.HandleAction(ma);

            
            // Assert
            
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.True(0.0f > b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }
        
        [Test]

        public void AddActionToQueue_ValidActionRightAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);
                
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.True(0.0f < b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            

        }
        
        [Test]

        public void AddActionToQueue_ValidActionUpAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";


            // Act
            gameLogic.HandleSpawnPlayer(
                username);
               
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Up;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.0f > b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            

        }

        [Test]

        public void AddActionToQueue_ValidActionDownAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";


            // Act
            gameLogic.HandleSpawnPlayer(
                username);

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Down;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.0f < b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            
        }
        
        [Test]

        public void AddActionToQueue_TwoValidActionUpRightAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);
            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);
            Thread.Sleep(300);

            MovementAction ma2 = new MovementAction();
            ma2.Direction = EDirection.Up;
            ma2.Token = null;
            ma2.Username = username;
            gameLogic.HandleAction(ma2);

            // Assert
            Thread.Sleep(300);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.True(0.0f < b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.0f > b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);

        }

        
        [Test]

        public void AddActionToQueue_ValidActionDownToNegativeAndPlayer_PositionMovesOnNextServerTick()
        {
            // Arrange
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);
            MovementAction ma = new MovementAction();
            //Phaser coordinates have up be negative
            ma.Direction = EDirection.Up;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.0f > b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }
        
        
        [Test]

        public void SpawnTwoPlayers_DifferentServerTick()
        {
            // See if game can suppost spawning in two players in different ticks

            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);

            Thread.Sleep(100);
       
            // Arrange 2
            string username2 = "realUser2";
            

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check P1
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);

            
            // Check P2
            Assert.True(b.ContainsKey("realUser2"));
            Assert.AreEqual(0.0f, b.Values.ElementAt(1).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(1).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }
        
        [Test]

        public void SpawnTwoPlayers_SameServerTick()
        {
            // See if the game can support spawning in two players at the same time
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);


            // Arrange 2
            string username2 = "realUser2";
            

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2);

            // Assert
            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check P1
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(0).ObjectBody.GetWorldPoint(Vector2.Zero).Y);


            // Check P2
            Assert.True(b.ContainsKey("realUser2"));
            Assert.AreEqual(0.0f, b.Values.ElementAt(1).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(1).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }
        
        /*
        [Test]

        public void SpawnTwoPlayers_DespawnMostRecentlySpawned()
        {
            // Spawn in two people and try despawning the one of them
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            float xPos = 0.0f;
            float yPos = 0.0f;

            // Act
            gameLogic.HandleSpawnPlayer(
                username,
                xPos,
                yPos);

            Thread.Sleep(100);

            // Arrange 2
            string username2 = "realUser2";
            float xPos2 = 0.0f;
            float yPos2 = 0.0f;

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);

            gameLogic.DespawnPlayer("realUser2");

            Thread.Sleep(200);
            var (a, b) = gameLogic.GetState();

            // Check if only P1 remains
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser", b.ElementAt(0).Key);
            Assert.AreEqual(0.0f, b.ElementAt(0).Value.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.ElementAt(0).Value.GetWorldPoint(Vector2.Zero).Y);

            
        }
        
        [Test]

        public void SpawnTwoPlayers_DespawnFirstPlayerSpawned()
        {
            // Spawn in two people and try despawning the one of them
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            float xPos = 0.0f;
            float yPos = 0.0f;

            // Act
            gameLogic.HandleSpawnPlayer(
                username,
                xPos,
                yPos);

            Thread.Sleep(100);

            // Arrange 2
            string username2 = "realUser2";
            float xPos2 = 0.0f;
            float yPos2 = 0.0f;

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2,
                xPos2,
                yPos2);

            // Assert
            Thread.Sleep(100);

            gameLogic.DespawnPlayer("realUser");

            Thread.Sleep(100);
            var (a, b) = gameLogic.GetState();

            // Check if only P2 remains
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual("realUser2", b.ElementAt(0).Key);

            Assert.AreEqual(0.0f, b.ElementAt(0).Value.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.ElementAt(0).Value.GetWorldPoint(Vector2.Zero).Y);



        }
        */

        [Test]
        
        public void SpawnTwoPlayers_ValidMovementActionFor1()
        {
            // Move the first player and not the second
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";


            // Act
            gameLogic.HandleSpawnPlayer(
                username);

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            Thread.Sleep(200);
            // Arrange 2
            string username2 = "realUser2";


            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2);

            // Assert
            Thread.Sleep(200);
            var (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.True(b.ContainsKey("realUser"));
            Assert.True(b.ContainsKey("realUser2"));

            //Users can be in either 0 or 1 spot so have to check first
            var usr1 = -1;
            var usr2 = -2;
            if(b.ElementAt(0).Key == "realUser")
            {
                usr1 = 0;
                usr2 = 1;
            }
            else
            {
                usr1 = 1;
                usr2 = 0;
            }
            // Check if P1 moved
            
            Assert.True(0.0f < b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            // Check if P2 is the same
            
            // P2 will have moved slightly
            Assert.True(-0.5f < b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.5f > b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).X);

            Assert.AreEqual(0.0f, b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).Y);

        }
        
        [Test]

        public void SpawnTwoPlayers_ValidMovementActionFor2()
        {
            // Move the second player and not the first

            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            float xPos = 0.0f;
            float yPos = 0.0f;

            // Act
            gameLogic.HandleSpawnPlayer(
                username);



            Thread.Sleep(200);
            // Arrange 2
            string username2 = "realUser2";
            

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2);

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username2;
            gameLogic.HandleAction(ma);

            // Assert
            Thread.Sleep(200);
            var (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.True(b.ContainsKey("realUser"));
            Assert.True(b.ContainsKey("realUser2"));

            //Users can be in either 0 or 1 spot so have to check first
            var usr1 = -1;
            var usr2 = -2;
            if(b.ElementAt(0).Key == "realUser")
            {
                usr1 = 0;
                usr2 = 1;
            }
            else
            {
                usr1 = 1;
                usr2 = 0;
            }
            // Check if P2 moved
            
            Assert.True(0.0f < b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            // Check if P1 is the same
            
            // P1 will have moved slightly
            Assert.True(-0.5f < b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.True(0.5f > b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).X);

            Assert.AreEqual(0.0f, b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
        }
        
        [Test]

        public void SpawnTwoPlayers_ValidMovementActionForBothPlayers()
        {
            // Move both players in different directions
            // Arrange 1
            var gameLogic = this.CreateGameLogic();
            string username = "realUser";
            

            // Act
            gameLogic.HandleSpawnPlayer(
                username);

            MovementAction ma = new MovementAction();
            ma.Direction = EDirection.Right;
            ma.Token = null;
            ma.Username = username;
            gameLogic.HandleAction(ma);

            Thread.Sleep(200);
            // Arrange 2
            string username2 = "realUser2";
            

            // Act 2
            gameLogic.HandleSpawnPlayer(
                username2);

            MovementAction ma2 = new MovementAction();
            ma2.Direction = EDirection.Left;
            ma2.Token = null;
            ma2.Username = username2;
            gameLogic.HandleAction(ma2);

            // Assert
            Thread.Sleep(200);
            var (a, b) = gameLogic.GetState();
            Assert.True(a.IsNullOrEmpty());
            Assert.AreEqual(2, b.Count);
            Assert.True(b.ContainsKey("realUser"));
            Assert.True(b.ContainsKey("realUser2"));

            //Users can be in either 0 or 1 spot so have to check first
            var usr1 = -1;
            var usr2 = -2;
            if (b.ElementAt(0).Key == "realUser")
            {
                usr1 = 0;
                usr2 = 1;
            }
            else
            {
                usr1 = 1;
                usr2 = 0;
            }
            // Check if P1 moved

            Assert.True(0.0f < b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(usr1).ObjectBody.GetWorldPoint(Vector2.Zero).Y);
            // Check if P2 is the same

            // P2 will have moved slightly
            Assert.True(0.0f >  b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).X);
            Assert.AreEqual(0.0f, b.Values.ElementAt(usr2).ObjectBody.GetWorldPoint(Vector2.Zero).Y);

        }
        

    }
}

    
