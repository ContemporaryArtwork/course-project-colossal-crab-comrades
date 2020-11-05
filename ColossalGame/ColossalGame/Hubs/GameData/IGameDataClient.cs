using ColossalGame.Hubs.DTO;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.GameData
{
    public interface IGameDataClient
    {
        Task ReceiveString(string message);
        Task ReceiveMessage(ASignalRDTO message);
        Task ReceiveToken(TokenMessageDTO message);
    }
}
