using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class CrouchStrategy : BaseStrategy
    {
        private float _crouchHeight;
        private Vector2 _crouchOffset;
        private float _moveSpeed;

        public CrouchStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {

            if (settings is CrouchSettings crouchingSettings)
            {
                _crouchHeight = crouchingSettings.CrouchHeight;
                _crouchOffset = crouchingSettings.CrouchOffset;
                _moveSpeed = crouchingSettings.MoveSpeed;
            }
        }
        public override void Execute()
        {

            // Perform crouching logic
            var collider = _locomotionController.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(collider.size.x, _crouchHeight);
            collider.offset = _crouchOffset;
            _locomotionController.RigidBody.linearVelocity = new Vector2(_inputHandler.MovementInput.x * _moveSpeed, _locomotionController.RigidBody.linearVelocity.y);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
