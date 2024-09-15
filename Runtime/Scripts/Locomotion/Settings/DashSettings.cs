using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "DashingSettings", menuName = "Locomotion/DashingSettings")]
    public class DashSettings : BaseSettings
    {
        [SerializeField] private float _dashForce = 20f;
        [SerializeField] private float _dashTime = 0.2f;

        private bool _forceApplied;

        public float dashForce => _dashForce;
        public float dashTime => _dashTime;


        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _forceApplied = false;
            _enterCondition = () => inputHandler.DashPressedThisFrame;

            bool exitToIdle() => controller.IsGrounded && _forceApplied && inputHandler.MovementInput.sqrMagnitude == 0f;
            bool exitToMove() => controller.IsGrounded && _forceApplied && inputHandler.MovementInput.sqrMagnitude > 0f;
            bool exitToJump() => controller.ApplyGravity && (controller.IsGrounded || controller.InCoyoteTime) && _forceApplied && inputHandler.JumpPressedThisFrame;

            _exitCondition = () => exitToIdle() || exitToMove() || exitToJump();

            return new DashStrategy(controller, inputHandler, this);
        }

        public void SetForceApplied(bool value)
        {
            _forceApplied = value;
        }

    }
}
