using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class WallClimbStrategy : BaseStrategy
    {
        private float wallClimbSpeed;
        private float staminaDrainRate;
        private Vector2 moveInput;

        public WallClimbStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {

            if (settings is WallClimbSettings wallClimbingSettings)
            {
                wallClimbSpeed = wallClimbingSettings.wallClimbSpeed;
                staminaDrainRate = wallClimbingSettings.staminaDrainRate;
            }
        }
        public override void Execute()
        {

            moveInput = _inputHandler.MovementInput;

            if (moveInput.y > 0 && _locomotionController.RigidBody.linearVelocity.y > 0)
            {
                _locomotionController.RigidBody.linearVelocity = new Vector2(_locomotionController.RigidBody.linearVelocity.x, wallClimbSpeed);
            }
        }
    }
}
