using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "WallClimbingSettings", menuName = "Locomotion/WallClimbingSettings")]
    public class WallClimbSettings : BaseSettings
    {
        [SerializeField] private float _wallClimbSpeed = 5f;
        [SerializeField] private float _duration = 2f;
        private bool _canSlide;
        public float WallClimbSpeed => _wallClimbSpeed;
        public float Duration => _duration;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _canSlide = false;
            _enterCondition = () => inputHandler.InteractPressedThisFrame && controller.IsTouchingWall && (inputHandler.MovementInput.y != 0f);

            bool exitToIdle() => controller.IsGrounded && controller.GetVelocity().sqrMagnitude == 0f;

            bool exitToFall() => controller.ApplyGravity && !controller.IsGrounded && (controller.GetVelocity().sqrMagnitude == 0f || !controller.IsTouchingWall) && !inputHandler.JumpPressedThisFrame;

            bool exitToWallJump() => controller.IsTouchingWall && inputHandler.JumpPressedThisFrame;

            bool exitToWallSlide() => controller.IsTouchingWall && !controller.IsGrounded && inputHandler.MovementInput.y == 0 && _canSlide;

            _exitCondition = () => exitToIdle() || exitToWallJump() || exitToFall() || exitToWallSlide();

            return new WallClimbStrategy(controller, inputHandler, this);
        }

        public void CanSlide(bool value) => _canSlide = value;

    }
}