using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class MoveStrategy : BaseStrategy
    {
        private Vector2 _inputDirection;
        private Vector2 _velocity;
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


            // Handle vertical movement if ApplyGravity is disabled
            if (!_locomotionController.ApplyGravity)
            {
                bool isChangingDirectionY = ShouldApplyTurnDeceleration(_velocity.y, _inputDirection.y);

                if (_inputDirection.y != 0)
                {
                    if (isChangingDirectionY)
                    {
                        // Apply turn deceleration for vertical
                        _velocity.y = Mathf.MoveTowards(
                            _velocity.y,
                            0,
                            moveSettings.TurnDeceleration * deltaTime
                        );
                    }
                    else
                    {
                        // Normal acceleration for vertical
                        float accelerationY = moveSettings.Acceleration * (isChangingDirectionY ? moveSettings.TurnAccelerationMultiplier : 1f);
                        _velocity.y = Mathf.MoveTowards(
                            _velocity.y,
                            _inputDirection.y * moveSettings.MaxSpeed,
                            accelerationY * deltaTime
                        );
                    }
                }
                else
                {
                    // Standard deceleration for vertical
                    _velocity.y = Mathf.MoveTowards(
                        _velocity.y,
                        0,
                        moveSettings.Deceleration * deltaTime
                    );
                }
            }

            _locomotionController.UpdatePlayerRotation(_inputDirection);
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

            // Preserve vertical velocity from other systems only if ApplyGravity is enabled
            if (_locomotionController.ApplyGravity)
            {
                float preservedY = _locomotionController.GetVelocity().y;
                _velocity.y = preservedY;
            }

            _locomotionController.SetVelocity(_velocity);

            // Reset Y velocity if grounded and ApplyGravity is enabled
            if (_locomotionController.IsGrounded && _locomotionController.ApplyGravity)
            {
                _velocity.y = 0;
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}