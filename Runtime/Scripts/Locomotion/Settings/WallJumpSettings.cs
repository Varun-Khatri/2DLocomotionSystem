using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "WallJumpSettings", menuName = "Locomotion/WallJumpSettings")]
    public class WallJumpSettings : BaseSettings
    {
        [Header("Jump Configuration")]
        [SerializeField]
        [Tooltip("Horizontal push force away from the wall")]
        private float _horizontalForce = 15f;

        [SerializeField]
        [Tooltip("Vertical upward jump force")]
        private float _verticalForce = 20f;

        [SerializeField]
        [Tooltip("Duration before regaining control")]
        private float _jumpDuration = 0.3f;

        private bool _canFall = false;
        public float HorizontalForce => _horizontalForce;
        public float VerticalForce => _verticalForce;
        public float JumpDuration => _jumpDuration;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _canFall = false;
            _enterCondition = () => inputHandler.JumpPressedThisFrame && controller.IsTouchingWall;

            bool exitToIdle() => controller.IsGrounded && inputHandler.MovementInput.sqrMagnitude == 0f;

            bool exitToMove() => controller.IsGrounded && inputHandler.MovementInput.x != 0;

            bool exitToWallClimb() => controller.IsTouchingWall && inputHandler.MovementInput.y > 0f;

            bool exitToDash() => inputHandler.DashPressedThisFrame;

            bool exitToFall() => !controller.IsGrounded && _canFall;

            _exitCondition = () => exitToIdle() || exitToMove() || exitToDash() || exitToWallClimb() || exitToFall();

            return new WallJumpStrategy(controller, inputHandler, this);
        }

        public void CanFall(bool value) { _canFall = value; }

    }
}