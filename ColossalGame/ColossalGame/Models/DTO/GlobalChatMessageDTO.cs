namespace ColossalGame.Models.DTO
{
    public class GlobalChatMessageDTO: IPayload
    {
        public string type { get; set; }
        
        public GlobalChatMessage message { get; set; }
    }
}