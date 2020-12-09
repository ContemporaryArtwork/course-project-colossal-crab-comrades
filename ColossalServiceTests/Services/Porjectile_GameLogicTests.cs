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
using ColossalGame.Models.GameModels;

namespace ColossalServiceTests.Services
{
    [TestFixture]
    public class Projectile_GameLogicTests
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
        public void SpawnPlayer_AddProjectile()
        {

            var gameLogic = this.CreateGameLogic();
            string username = "realUser";

            gameLogic.HandleSpawnPlayer(username);

            ShootingAction s = new ShootingAction();
            s.Angle = 0.0f;
            s.Username = "realUser";
            
            gameLogic.HandleAction(s);

            var (a, b) = gameLogic.GetState();

            Assert.AreEqual(1, a.Values.Count);
           
        }

        [Test]
        public void SpawnPlayer_AddProjectile_Multiple_SameUser()
        {

            var gameLogic = this.CreateGameLogic();
            string username = "realUser";

            gameLogic.HandleSpawnPlayer(username);

            ShootingAction s = new ShootingAction();
            s.Angle = 0.0f;
            s.Username = "realUser";

            gameLogic.HandleAction(s);
            gameLogic.HandleAction(s);
            gameLogic.HandleAction(s);

            var (a, b) = gameLogic.GetState();

            Assert.AreEqual(3, a.Values.Count);

        }

        [Test]
        public void SpawnPlayer_AddProjectile_MultipleLarge_SameUser()
        {

            var gameLogic = this.CreateGameLogic();
            string username = "realUser";

            gameLogic.HandleSpawnPlayer(username);

            ShootingAction s = new ShootingAction();
            s.Angle = 0.0f;
            s.Username = "realUser";

            for(int i =0; i< 100; i++)
            {
                gameLogic.HandleAction(s);
            }
            
            

            var (a, b) = gameLogic.GetState();

            Assert.AreEqual(100, a.Values.Count);

        }

        

        [Test]
        public void AddProjectile_CorrectPosition()
        {

            var gameLogic = this.CreateGameLogic();
            string username = "realUser";

            gameLogic.HandleSpawnPlayer(username);

            ShootingAction s = new ShootingAction();
            s.Angle = 0.0f;
            s.Username = "realUser";

            gameLogic.HandleAction(s);

            var (a, b) = gameLogic.GetState();

            
            Assert.AreEqual(0.0f, a.ElementAt(0).Value.XPos);

            Assert.AreEqual(0.0f, a.ElementAt(0).Value.YPos);

        }

        [Test]
        public void AddProjectile_CorrectPosition_CorrectAngle()
        {

            var gameLogic = CreateGameLogic();
            string username = "realUser";

            gameLogic.HandleSpawnPlayer(username);

            ShootingAction s = new ShootingAction();
            s.Angle = 0.0f;
            s.Username = "realUser";

            gameLogic.HandleAction(s);

            var (a, b) = gameLogic.GetState();

            
            //Changed in the BulletModel Class from 0
            Assert.AreEqual(19.2f, a.ElementAt(0).Value.Radius);

        }

    }
}
