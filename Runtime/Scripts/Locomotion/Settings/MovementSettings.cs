using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Locomotion/MovementSettings")]
    public class MovementSettings : BaseSettings
    {
        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _acceleration = 5f;
        [SerializeField] private float _deceleration = 5f;
        [Header("Direction Change Settings")]
        [SerializeField] private float _turnDeceleration = 40f;  // Higher deceleration when changing directions
        [SerializeField] private float _turnAccelerationMultiplier = 1.5f;  // Acceleration boost after direction change

        public float MaxSpeed => _maxSpeed;
        public float Deceleration => _deceleration;
        public float Acceleration => _acceleration;
        public float TurnDeceleration => _turnDeceleration;
        public float TurnAccelerationMultiplier => _turnAccelerationMultiplier;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () =>
            {
                if (controller.ApplyGravity)
                    return controller.IsGrounded && inputHandler.MovementInput.y == 0f && (inputHandler.MovementInput.magnitude > 0f || controller.GetVelocity().magnitude > 0f);
                else
                    return (inputHandler.MovementInput.magnitude > 0f || controller.GetVelocity().magnitude > 0f);
            };

            bool exitToIdle()
            {
                if (controller.ApplyGravity)
                    return controller.IsGrounded && controller.GetVelocity().sqrMagnitude == 0 && inputHandler.MovementInput.sqrMagnitude == 0f;
                else
                    return controller.GetVelocity().sqrMagnitude == 0 && inputHandler.MovementInput.sqrMagnitude == 0f;
            };

            bool exitToJump() => controller.ApplyGravity && (controller.IsGrounded || controller.InCoyoteTime) && inputHandler.JumpPressedThisFrame;

            bool exitToDash() => (controller.IsGrounded || controller.InCoyoteTime) && inputHandler.DashPressedThisFrame;

            bool exitToWallClimb() => controller.IsTouchingWall && inputHandler.MovementInput.y > 0f;

            bool exitToFall() => controller.ApplyGravity && !controller.IsGrounded;


            _exitCondition = () => exitToIdle() || exitToJump() || exitToDash() || exitToWallClimb() || exitToFall();

            return new MoveStrategy(controller, inputHandler, this);
        }
    }
}