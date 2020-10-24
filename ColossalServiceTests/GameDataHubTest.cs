using System;
using System.Threading.Tasks;
using SignalR_UnitTestingSupport.Hubs;
using NUnit.Framework;
using ColossalGame.Models.Hubs.Clients;
using ColossalGame.Models.Hubs;
using Moq;
using ColossalGame.Models;

namespace ColossalServiceTests
{
    [TestFixture]
    public class GameDataHubTest : HubUnitTestsBase<IGameDataClient>
    {
        [Test]
        public async Task CallChangeWeapon()
        {
            var connId = Guid.NewGuid().ToString();

            var exampleHub = new GameDataHub();
            AssignToHubRequiredProperties(exampleHub);

            ContextMock.Setup(x => x.Items).Returns(ItemsFake);

            //Conn id is guid. It is different in each test. But is same in scope of test.
            ContextMock.Setup(x => x.ConnectionId).Returns(connId);

            await exampleHub.ChangeWeapon("New Weapon");

            ClientsAllMock.Verify(
                x=> x.ReceiveString("This was your message: New Weapon"),
                Times.Once()
                );
        }
        [Test]
        public async Task CallSendMovement()
        {
            var connId = Guid.NewGuid().ToString();

            var exampleHub = new GameDataHub();
            AssignToHubRequiredProperties(exampleHub);

            ContextMock.Setup(x => x.Items).Returns(ItemsFake);

            //Conn id is guid. It is different in each test. But is same in scope of test.
            ContextMock.Setup(x => x.ConnectionId).Returns(connId);

            await exampleHub.SendMovement(new MovementAction { Username="", Token="", Direction=EDirection.Right});

            ClientsCallerMock.Verify(
                x => x.ReceiveString("Action rejected by interpolator. Action sent too close to previous action."),
                Times.Once()
                );
        }
        [Test]
        public async Task CallExitGame()
        {
            var connId = Guid.NewGuid().ToString();

            var exampleHub = new GameDataHub();
            AssignToHubRequiredProperties(exampleHub);

            ContextMock.Setup(x => x.Items).Returns(ItemsFake);

            //Conn id is guid. It is different in each test. But is same in scope of test.
            ContextMock.Setup(x => x.ConnectionId).Returns(connId);

            await exampleHub.ExitGame("quit");

            ClientsAllMock.Verify(
                x => x.ReceiveString("This was your message: quit"),
                Times.Once()
                );
        }
        [Test]
        public async Task CallFireWeapon()
        {
            var connId = Guid.NewGuid().ToString();

            var exampleHub = new GameDataHub();
            AssignToHubRequiredProperties(exampleHub);

            ContextMock.Setup(x => x.Items).Returns(ItemsFake);

            //Conn id is guid. It is different in each test. But is same in scope of test.
            ContextMock.Setup(x => x.ConnectionId).Returns(connId);

            await exampleHub.FireWeapon("shoot");

            ClientsAllMock.Verify(
                x => x.ReceiveString("This was your message: shoot"),
                Times.Once()
                );
        }
    }
}
