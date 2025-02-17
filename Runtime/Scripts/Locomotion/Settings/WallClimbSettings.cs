using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "WallClimbingSettings", menuName = "Locomotion/WallClimbingSettings")]
    public class WallClimbSettings : BaseSettings
    {
        public float wallClimbSpeed = 5f;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () =>
            {
                return controller.IsTouchingWall && (inputHandler.MovementInput.y != 0f);
            };

            bool exitToIdle()
            {
                return controller.IsGrounded && (controller.GetVelocity().sqrMagnitude == 0f || !controller.IsTouchingWall);
            };

            _exitCondition = () => exitToIdle();

            return new WallClimbStrategy(controller, inputHandler, this);
        }
    }
}