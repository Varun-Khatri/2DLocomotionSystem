using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class JumpStrategy : BaseStrategy
    {
        private bool _hasReachedApex;
        private Vector2 _velocity;
        private bool _jumpComplete;
        private float _startHeight;
        private float _baseGravity; // Store base gravity computed from jump parameters

        public JumpStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {
            _hasReachedApex = false;
        }

        public override void Enter()
        {
            base.Enter();
            _hasReachedApex = false;
            var jumpSettings = (JumpSettings)_settings;
            jumpSettings.SetApex(_hasReachedApex);
            jumpSettings.SetCompletion(_jumpComplete = false);
            _locomotionController.ResetCoyoteTime();
            _velocity = _locomotionController.GetVelocity();
            ApplyJumpForce();
            _startHeight = _locomotionController.transform.position.y;
        }

        private void ApplyJumpForce()
        {
            var jumpSettings = (JumpSettings)_settings;
            // Calculate gravity based on desired jump height and time to apex
            _baseGravity = (2 * jumpSettings.JumpHeight) / Mathf.Pow(jumpSettings.TimeToApex, 2);
            // Calculate initial jump velocity
            float jumpForce = Mathf.Sqrt(2 * _baseGravity * jumpSettings.JumpHeight);
            _velocity.y = jumpForce;
            _locomotionController.SetVelocity(_velocity);
        }

        public override void Execute()
        {
            base.Execute();
            var jumpSettings = (JumpSettings)_settings;

            // Get current velocity once per frame
            Vector2 currentVelocity = _locomotionController.GetVelocity();

            // Calculate gravity effects
            float currentGravity = _baseGravity;
            if (currentVelocity.y > 0 && !_inputHandler.HoldingJump)
            {
                currentGravity *= jumpSettings.LowJumpMultiplier;
            }
            else if (currentVelocity.y < 0)
            {
                currentGravity *= jumpSettings.FallMultiplier;
            }

            // Apply gravity with deltaTime
            currentVelocity.y -= currentGravity * Time.deltaTime;

            // Horizontal control with proper acceleration
            float targetHorizontalSpeed = _inputHandler.MovementInput.x * jumpSettings.MaxHorizontalSpeed;
            currentVelocity.x = Mathf.MoveTowards(
                currentVelocity.x,
                targetHorizontalSpeed,
                jumpSettings.HorizontalAcceleration * Time.deltaTime
            );

            // Clamp vertical speed
            currentVelocity.y = Mathf.Max(currentVelocity.y, -jumpSettings.MaxFallSpeed);

            // Update velocity
            _locomotionController.SetVelocity(currentVelocity);

            // Apex detection
            if (!_hasReachedApex && currentVelocity.y <= 0)
            {
                _hasReachedApex = true;
                jumpSettings.SetApex(_hasReachedApex);
            }

            // Jump completion check
            if (_hasReachedApex && _startHeight > _locomotionController.transform.position.y)
            {
                jumpSettings.SetCompletion(_jumpComplete = true);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}