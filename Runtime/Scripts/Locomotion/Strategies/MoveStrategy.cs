using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class MoveStrategy : BaseStrategy
    {
        private Vector2 _inputDirection;
        private Vector2 _velocity;
        private float _cachedDirection;
        public MoveStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings) { }

        public override void Enter()
        {
            base.Enter();
            _velocity = _locomotionController.GetVelocity();
        }

        public override void Execute()
        {
            base.Execute();
            _inputDirection = _inputHandler.MovementInput;

            // Handle horizontal movement
            if (_inputDirection.x != 0)
            {
                _velocity.x += _inputDirection.x * ((MovementSettings)_settings).Acceleration;
            }
            else if (_velocity.x != 0)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, ((MovementSettings)_settings).Deceleration);
            }
            // Clamp the velocity to max speed
            _velocity.x = Mathf.Clamp(_velocity.x, -((MovementSettings)_settings).MaxSpeed, ((MovementSettings)_settings).MaxSpeed);

            if (!_locomotionController.ApplyGravity)
            {
                if (_inputDirection.y != 0)
                {
                    _velocity.y += _inputDirection.y * ((MovementSettings)_settings).Acceleration;
                }
                else if (_velocity.y != 0)
                {
                    _velocity.y = Mathf.MoveTowards(_velocity.y, 0, ((MovementSettings)_settings).Deceleration);
                }

                _velocity.y = Mathf.Clamp(_velocity.y, -((MovementSettings)_settings).MaxSpeed, ((MovementSettings)_settings).MaxSpeed);
            }

            if (_locomotionController.ApplyGravity)
            {
                _velocity.y = _locomotionController.LocomotionSettings.gravity;
            }

            HandleRotation();
        }

        public override void PhysicsExecute()
        {
            base.PhysicsExecute();

            // Apply the computed velocity to the player controller
            _locomotionController.SetVelocity(_velocity * Time.fixedDeltaTime);

            if (_locomotionController.ApplyGravity && _locomotionController.IsGrounded)
            {
                _velocity.y = 0;
            }
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

        public override void Exit()
        {
            base.Exit();
        }
    }
}