using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class MoveStrategy : BaseStrategy
    {
        private Vector2 _inputDirection;
        private Vector2 _velocity;
        private float _cachedDirection;
        private bool _isChangingDirection;

        public MoveStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings) { }

        public override void Enter()
        {
            base.Enter();
            _velocity = _locomotionController.GetVelocity();
            _isChangingDirection = false;
        }

        public override void Execute()
        {
            base.Execute();
            _inputDirection = _inputHandler.MovementInput;

            float deltaTime = Time.deltaTime;
            var moveSettings = (MovementSettings)_settings;

            // Calculate direction change state
            _isChangingDirection = ShouldApplyTurnDeceleration(_velocity.x, _inputDirection.x);

            // Handle horizontal movement
            if (_inputDirection.x != 0)
            {
                if (_isChangingDirection)
                {
                    // Apply turn deceleration first
                    _velocity.x = Mathf.MoveTowards(
                        _velocity.x,
                        0,
                        moveSettings.TurnDeceleration * deltaTime
                    );
                }
                else
                {
                    // Normal acceleration
                    float acceleration = moveSettings.Acceleration * (_isChangingDirection ? moveSettings.TurnAccelerationMultiplier : 1f);
                    _velocity.x = Mathf.MoveTowards(
                        _velocity.x,
                        _inputDirection.x * moveSettings.MaxSpeed,
                        acceleration * deltaTime
                    );
                }
            }
            else
            {
                // Standard deceleration
                _velocity.x = Mathf.MoveTowards(
                    _velocity.x,
                    0,
                    moveSettings.Deceleration * deltaTime
                );
            }

            HandleRotation();
        }

        private bool ShouldApplyTurnDeceleration(float currentVelocityX, float inputX)
        {
            return !Mathf.Approximately(inputX, 0) &&
                   Mathf.Sign(currentVelocityX) != Mathf.Sign(inputX) &&
                   Mathf.Abs(currentVelocityX) > 0.1f;
        }

        public override void PhysicsExecute()
        {
            base.PhysicsExecute();
            // Maintain vertical velocity from other systems
            float preservedY = _locomotionController.GetVelocity().y;
            _velocity.y = preservedY;

            _locomotionController.SetVelocity(_velocity);

            if (_locomotionController.IsGrounded)
            {
                _velocity.y = 0;
            }
        }

        private void HandleRotation()
        {
            if (_inputDirection.x == 0 || Mathf.Approximately(_inputDirection.x, _cachedDirection)) return;

            _cachedDirection = _inputDirection.x;
            bool facingRight = _inputDirection.x > 0;
            _locomotionController.SetRotation(facingRight ?
                Quaternion.identity :
                Quaternion.Euler(0, 180, 0)
            );
            _locomotionController.SetFacing(facingRight);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}