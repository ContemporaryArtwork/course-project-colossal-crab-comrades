using ColossalGame.Hubs.DTO;
using System.Threading.Tasks;

namespace ColossalGame.Hubs.Chat
{
    public interface IChatClient
    {
        Task ReceiveMessage(GlobalChatMessageDTO message);
    }
}
