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

        public float HorizontalForce => _horizontalForce;
        public float VerticalForce => _verticalForce;
        public float JumpDuration => _jumpDuration;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () => inputHandler.JumpPressedThisFrame && controller.IsTouchingWall;
            _exitCondition = () => controller.IsGrounded || controller.IsTouchingWall;

            return new WallJumpStrategy(controller, inputHandler, this);
        }
    }
}