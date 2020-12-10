namespace ColossalGame.Models.GameModels
{
    public class ShootingAction : AUserAction
    {
        /// <summary>
        /// Angle of shot. Needs to be in RADIANS!!
        /// </summary>
        private float secretAngle { get; set; }
        public float Angle
        {
            get => secretAngle;
            set => secretAngle = value;
        }
    }
}
