namespace ColossalGame.Models.GameModels
{
    public enum EDirection
    {
        Down,
        Up,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public class AUserAction
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }

    public class MovementAction : AUserAction
    {
        
        public EDirection Direction { get; set; }
        

    }

    public class SpawnAction: AUserAction
    {
        
    }



    
}
