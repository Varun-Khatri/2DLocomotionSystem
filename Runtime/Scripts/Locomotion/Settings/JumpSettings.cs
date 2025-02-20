using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "JumpingSettings", menuName = "Locomotion/JumpingSettings")]
    public class JumpSettings : BaseSettings
    {
        [Header("Jump Settings")]
        [SerializeField] private float _jumpHeight = 4f; // Desired jump height
        [SerializeField] private float _timeToApex = 0.4f; // Time to reach the apex of the jump
        [SerializeField] private float _horizontalControl = 10f; // Horizontal speed control

        [Header("Gravity Settings")]
        [SerializeField] private float _fallMultiplier = 2.5f; // Multiplier for falling speed
        [SerializeField] private float _lowJumpMultiplier = 2f; // Multiplier for low jumps (e.g., if jump is released early)
        [SerializeField] private float _maxFallSpeed = 10f; // Maximum falling speed before applying increased gravity

        private bool _reachedApex = false;
        private bool _jumpComplete = false;
        public float JumpHeight => _jumpHeight;
        public float TimeToApex => _timeToApex;
        public float HorizontalControl => _horizontalControl;

        public float FallMultiplier => _fallMultiplier;
        public float LowJumpMultiplier => _lowJumpMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _reachedApex = false;

            _enterCondition = () => controller.ApplyGravity && (controller.IsGrounded || controller.InCoyoteTime) && inputHandler.JumpPressedThisFrame;

            bool exitToIdle() => controller.IsGrounded && _reachedApex && inputHandler.MovementInput.sqrMagnitude == 0f;
            bool exitToMove() => controller.IsGrounded && _reachedApex && inputHandler.MovementInput.sqrMagnitude > 0f;

            bool exitToDash() => inputHandler.DashPressedThisFrame;

            bool exitToWallClimb() => controller.IsTouchingWall && inputHandler.MovementInput.y > 0f;

            bool exitToFall() => controller.ApplyGravity && !controller.IsGrounded && _jumpComplete;

            _exitCondition = () => exitToIdle() || exitToMove() || exitToDash() || exitToWallClimb() || exitToFall();

            return new JumpStrategy(controller, inputHandler, this);
        }

        public float GetDynamicGravityScale(float currentVelocityY)
        {
            return _fallMultiplier * Mathf.Clamp(currentVelocityY / _maxFallSpeed, 1, _lowJumpMultiplier);
        }

        public void SetApex(bool value)
        {
            _reachedApex = value;
        }

        public void SetCompletion(bool value)
        {
            _jumpComplete = value;
        }

    }
}