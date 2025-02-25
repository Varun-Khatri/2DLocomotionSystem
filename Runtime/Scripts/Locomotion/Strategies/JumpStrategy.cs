using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class JumpStrategy : BaseStrategy
    {
        private bool _hasReachedApex;
        private Vector2 _velocity;
        private float _initialGravityScale;
        private float _jumpStartTime;
        private bool _jumpComplete;
        private float _startHeight;

        public JumpStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {
            _hasReachedApex = false;
            _initialGravityScale = locomotionController.RigidBody.gravityScale;
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
            _jumpStartTime = Time.time;
            _startHeight = _locomotionController.transform.position.y;
            Debug.Log("Entering Jump Strategy");
        }

        private void ApplyJumpForce()
        {
            var jumpSettings = (JumpSettings)_settings;
            float gravity = (2 * jumpSettings.JumpHeight) / Mathf.Pow(jumpSettings.TimeToApex, 2);
            float jumpForce = Mathf.Sqrt(2 * gravity * jumpSettings.JumpHeight);

            _velocity.y = jumpForce;
            _locomotionController.SetVelocity(_velocity);

            _locomotionController.SetGravityScale(gravity / Physics2D.gravity.y);
        }

        public override void Execute()
        {
            base.Execute();
            var jumpSettings = (JumpSettings)_settings;

            _velocity = _locomotionController.GetVelocity();

            // Check if the player has reached the apex of the jump
            if (!_hasReachedApex && _velocity.y <= 0)
            {
                _hasReachedApex = true;
                ((JumpSettings)_settings).SetApex(_hasReachedApex);
            }

            if (_hasReachedApex && _startHeight > _locomotionController.transform.position.y)
            {
                jumpSettings.SetCompletion(_jumpComplete = true);
            }

            // Apply gravity adjustments for Celeste-like jump
            if (_velocity.y > 0)
            {
                // Ascending phase: use normal gravity
                _locomotionController.SetGravityScale(_locomotionController.LocomotionSettings.gravityScale);
            }
            else if (_velocity.y < 0)
            {
                // Descending phase: apply dynamic gravity based on velocity
                float dynamicGravityScale = jumpSettings.FallMultiplier * Mathf.Clamp(_velocity.y / jumpSettings.MaxFallSpeed, 1, jumpSettings.LowJumpMultiplier);
                _locomotionController.SetGravityScale(dynamicGravityScale);

            }

            // Apply horizontal control during the jump
            _velocity.x = _inputHandler.MovementInput.x * jumpSettings.HorizontalControl;
            _locomotionController.SetVelocity(_velocity);

            // Check for early jump release
            if (!_inputHandler.HoldingJump && Time.time - _jumpStartTime > jumpSettings.TimeToApex * 0.5f)
            {
                // Apply early jump release behavior (e.g., reduce jump height)
                _velocity.y *= 0.5f; // Adjust as needed
            }
        }

        public override void Exit()
        {
            base.Exit();
            _locomotionController.SetGravityScale(_initialGravityScale); // Reset gravity scale when grounded
            Debug.Log("Exiting Jump");
        }
    }
}