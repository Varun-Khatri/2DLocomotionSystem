using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class FallStrategy : BaseStrategy
    {
        private Vector2 _inputDirection;
        private Vector2 _velocity;
        private float _cachedDirection;
        private float _enterScale;
        public FallStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
                   : base(locomotionController, inputHandler, settings) { }

        public override void Enter()
        {
            base.Enter();
            _velocity = _locomotionController.GetVelocity();
            _enterScale = _locomotionController.RigidBody.gravityScale;
            _locomotionController.SetGravityScale(((FallSettings)_settings).FallMultiplier);
            Debug.Log("Entering Fall Strategy");
        }

        public override void Exit()
        {
            base.Exit();
            _locomotionController.SetGravityScale(_enterScale);
        }

        public override void Execute()
        {
            base.Execute();
            ApplyGravity();
            ApplyMovement();
            HandleRotation();
        }

        public override void PhysicsExecute()
        {
            base.PhysicsExecute();
            // Apply the computed velocity to the player controller
            _locomotionController.SetVelocity(_velocity * Time.fixedDeltaTime);
        }

        private void ApplyGravity()
        {
            _velocity.y += _locomotionController.LocomotionSettings.gravity * ((FallSettings)_settings).FallMultiplier * Time.fixedDeltaTime;
            // Clamp the velocity to max speed
            _velocity.y = Mathf.Clamp(_velocity.y, -((FallSettings)_settings).MaxFallSpeed, ((FallSettings)_settings).MaxFallSpeed);
        }

        private void ApplyMovement()
        {
            _inputDirection = _inputHandler.MovementInput;
            // Handle horizontal movement
            if (_inputDirection.x != 0)
            {
                _velocity.x += _inputDirection.x * ((FallSettings)_settings).Acceleration;
            }
            else if (_velocity.x != 0)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, ((FallSettings)_settings).Deceleration);
            }
            // Clamp the velocity to max speed
            _velocity.x = Mathf.Clamp(_velocity.x, -((FallSettings)_settings).MaxMoveSpeed, ((FallSettings)_settings).MaxMoveSpeed);
        }

        private void HandleRotation()
        {
            if (_inputDirection.x != 0 && _inputDirection.x != _cachedDirection)
            {
                _locomotionController.SetRotation(_inputDirection.x > 0
                    ? Quaternion.Euler(0, 0, 0)  // No rotation (0 degrees)
                    : Quaternion.Euler(0, 180, 0)); // 180-degree rotation
                _locomotionController.SetFacing(_inputDirection.x > 0);

                _cachedDirection = _inputDirection.x;
            }
        }

    }
}
