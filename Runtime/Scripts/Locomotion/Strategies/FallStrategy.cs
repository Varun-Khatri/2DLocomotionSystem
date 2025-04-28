using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class FallStrategy : BaseStrategy
    {
        private Vector2 _inputDirection;
        private Vector2 _velocity;
        private float _cachedDirection;

        public FallStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings) { }

        public override void Enter()
        {
            base.Enter();
            _velocity = _locomotionController.GetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Execute()
        {
            base.Execute();
            // Handle input/rotation in Update for responsiveness
            _inputDirection = _inputHandler.MovementInput;
            _locomotionController.UpdatePlayerRotation(_inputDirection);
        }

        public override void PhysicsExecute()
        {
            base.PhysicsExecute();
            // Handle physics calculations in FixedUpdate
            ApplyGravity();
            ApplyMovement();

            // Apply final velocity to the controller
            _locomotionController.SetVelocity(_velocity);
        }

        private void ApplyGravity()
        {
            var fallSettings = (FallSettings)_settings;
            // Apply gravity acceleration (units/sec�)
            float gravity = _locomotionController.LocomotionSettings.gravity * fallSettings.FallMultiplier;
            _velocity.y += gravity * Time.fixedDeltaTime;

            // Clamp max fall speed
            _velocity.y = Mathf.Clamp(
                _velocity.y,
                -fallSettings.MaxFallSpeed,
                float.MaxValue // Allow upward velocity from external forces
            );
        }

        private void ApplyMovement()
        {
            var fallSettings = (FallSettings)_settings;

            // Horizontal movement
            if (_inputDirection.x != 0)
            {
                // Accelerate with input
                _velocity.x += _inputDirection.x * fallSettings.Acceleration * Time.fixedDeltaTime;
                // Clamp to max speed
                _velocity.x = Mathf.Clamp(
                    _velocity.x,
                    -fallSettings.MaxMoveSpeed,
                    fallSettings.MaxMoveSpeed
                );
            }
            else
            {
                // Decelerate to zero
                _velocity.x = Mathf.MoveTowards(
                    _velocity.x,
                    0,
                    fallSettings.Deceleration * Time.fixedDeltaTime
                );
            }
        }
    }
}