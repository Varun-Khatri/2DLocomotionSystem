using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "WallSlideSettings", menuName = "Locomotion/WallSlideSettings")]
    public class WallSlideSettings : BaseSettings
    {
        [SerializeField] private float _slideSpeed = -5f;
        public float SlideSpeed => _slideSpeed;
        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () => controller.IsTouchingWall && !controller.IsGrounded && inputHandler.MovementInput.y == 0f;

            bool exitToIdle() => controller.IsGrounded && controller.IsTouchingWall && inputHandler.MovementInput.x == 0f;
            bool exitToMove() => controller.IsGrounded && controller.IsTouchingWall && inputHandler.MovementInput.x != 0f;
            bool exitToFall() => !controller.IsTouchingWall && !controller.IsGrounded;
            bool exitToWallJump() => controller.IsTouchingWall && inputHandler.JumpPressedThisFrame;
            bool exitToWallClimb() => inputHandler.InteractPressedThisFrame && controller.IsTouchingWall && inputHandler.MovementInput.y != 0f;

            _exitCondition = () => exitToIdle() || exitToMove() || exitToFall() || exitToWallJump() || exitToWallClimb();

            return new WallSlideStrategy(controller, inputHandler, this);
        }
    }
}
