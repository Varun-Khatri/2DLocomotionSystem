using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "WallClimbingSettings", menuName = "Locomotion/WallClimbingSettings")]
    public class WallClimbSettings : BaseSettings
    {
        public float wallClimbSpeed = 5f;
        public float staminaDrainRate = 1f;
        public float wallCheckDistance = 0.5f;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            throw new System.NotImplementedException();
        }
    }
}