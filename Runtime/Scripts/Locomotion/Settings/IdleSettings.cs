using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "IdleSettings", menuName = "Locomotion/IdleSettings")]
    public class IdleSettings : BaseSettings
    {
        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            _enterCondition = () =>
            {
                if (controller.ApplyGravity)
                    return controller.IsGrounded && controller.GetVelocity().sqrMagnitude == 0f;
                else
                    return controller.GetVelocity().sqrMagnitude == 0f;
            };

            bool exitToMove()
            {
                if (controller.ApplyGravity)
                    return controller.IsGrounded && inputHandler.MovementInput.magnitude > 0f;
                else
                    return inputHandler.MovementInput.magnitude > 0f;
            }

            bool exitToJump() => controller.ApplyGravity && (controller.IsGrounded || controller.InCoyoteTime) && inputHandler.JumpPressedThisFrame;

            bool exitToDash() => (controller.IsGrounded || controller.InCoyoteTime) && inputHandler.DashPressedThisFrame;

            _exitCondition = () => exitToMove() || exitToJump() || exitToDash();


            return new IdleStrategy(this, controller, inputHandler);
        }
    }
}
