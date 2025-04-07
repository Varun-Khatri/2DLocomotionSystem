using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "FallSettings", menuName = "Locomotion/FallSettings")]
    public class FallSettings : BaseSettings
    {

        [SerializeField] private float _fallMultiplier = 2f;
        [SerializeField] private float _maxFallSpeed = 10f;
        [SerializeField] private float _maxMoveSpeed = 10f;
        [SerializeField] private float _acceleration = 5f;
        [SerializeField] private float _deceleration = 5f;

        public float FallMultiplier => _fallMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;
        public float MaxMoveSpeed => _maxMoveSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () =>
            {
                if (controller.ApplyGravity)
                    return !controller.IsGrounded;
                else
                    return false;
            };

            bool exitToIdle()
            {
                if (controller.ApplyGravity)
                {
                    return controller.IsGrounded;
                }
                else
                    return true;
            }
            ;

            bool exitToMove()
            {
                if (controller.ApplyGravity && controller.GetVelocity().x != 0f)
                {
                    return controller.IsGrounded;
                }
                else
                    return true;
            }
            ;

            bool exitToDash()
            {
                if (controller.ApplyGravity && controller.GetVelocity().x != 0f)
                {
                    return inputHandler.DashPressedThisFrame;
                }
                else
                    return true;
            }

            _exitCondition = () => exitToIdle() || exitToMove() || exitToDash();

            return new FallStrategy(controller, inputHandler, this);
        }
    }
}
