using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "DashingSettings", menuName = "Locomotion/DashingSettings")]
    public class DashSettings : BaseSettings
    {
        [SerializeField] private float _dashDistance = 5f;  // Total distance in units
        [SerializeField] private float _dashTime = 0.2f;    // Duration in seconds
        [SerializeField] private float _cooldown = 0.4f;

        private bool _forceApplied;
        public float dashDistance => _dashDistance;
        public float dashTime => _dashTime;
        public float cooldown => _cooldown;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () => inputHandler.DashPressedThisFrame;

            bool exitToIdle() => _forceApplied && inputHandler.MovementInput.sqrMagnitude == 0f;
            bool exitToMove() => _forceApplied && inputHandler.MovementInput.sqrMagnitude > 0f;
            bool exitToJump() => _forceApplied && inputHandler.JumpPressedThisFrame;

            _exitCondition = () => exitToIdle() || exitToMove() || exitToJump();

            return new DashStrategy(controller, inputHandler, this);
        }

        public void SetForceApplied(bool value)
        {
            _forceApplied = value;
        }
    }
}