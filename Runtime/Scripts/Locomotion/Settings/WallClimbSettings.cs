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
            _enterCondition = () => controller.IsTouchingWall && (inputHandler.MovementInput.y != 0f);

            bool exitToIdle() => controller.IsGrounded && (controller.GetVelocity().sqrMagnitude == 0f || !controller.IsTouchingWall);

            bool exitToFall() => controller.ApplyGravity && !controller.IsGrounded && (controller.GetVelocity().sqrMagnitude == 0f || !controller.IsTouchingWall) && !inputHandler.JumpPressedThisFrame;

            bool exitToWallJump() => controller.IsTouchingWall && inputHandler.JumpPressedThisFrame;

            _exitCondition = () => exitToIdle() || exitToWallJump() || exitToFall();

            return new WallClimbStrategy(controller, inputHandler, this);
        }
    }
}