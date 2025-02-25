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

            // Handle horizontal movement with deltaTime
            float deltaTime = Time.deltaTime;
            if (_inputDirection.x != 0)
            {
                _velocity.x += _inputDirection.x * ((MovementSettings)_settings).Acceleration * deltaTime;
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(
                    _velocity.x,
                    0,
                    ((MovementSettings)_settings).Deceleration * deltaTime
                );
            }

            _velocity.x = Mathf.Clamp(_velocity.x,
                -((MovementSettings)_settings).MaxSpeed,
                ((MovementSettings)_settings).MaxSpeed
            );

            // Remove vertical movement logic from MoveStrategy
            HandleRotation();
        }

        public override void PhysicsExecute()
        {
            base.PhysicsExecute();
            // Apply raw velocity without Time.fixedDeltaTime
            _locomotionController.SetVelocity(_velocity);

            // Let gravity be handled by the physics system or FallStrategy
            if (_locomotionController.IsGrounded)
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