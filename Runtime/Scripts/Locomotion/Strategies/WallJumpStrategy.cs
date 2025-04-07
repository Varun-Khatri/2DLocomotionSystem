using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class WallJumpStrategy : BaseStrategy
    {
        private readonly float _horizontalForce;
        private readonly float _verticalForce;
        private readonly float _jumpDuration;

        private float _horizontalDirection;
        private float _timer;

        public WallJumpStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {
            WallJumpSettings wallSettings = settings as WallJumpSettings;
            _horizontalForce = wallSettings.HorizontalForce;
            _verticalForce = wallSettings.VerticalForce;
            _jumpDuration = wallSettings.JumpDuration;
        }

        public override void Enter()
        {
            base.Enter();
            Vector2 wallNormal = _locomotionController.GetWallNormal();
            _horizontalDirection = Mathf.Sign(wallNormal.x);
            ApplyJumpForce();
        }

        private void ApplyJumpForce()
        {
            // Set initial velocity using calculated direction
            _locomotionController.SetVelocity(new Vector2(
                _horizontalDirection * _horizontalForce,
                _verticalForce
            ));
        }

        public override void Execute()
        {
            base.Execute();
            _timer += Time.deltaTime;

            if (_timer < _jumpDuration)
            {
                // Maintain horizontal velocity using pre-calculated direction
                Vector2 current = _locomotionController.GetVelocity();
                _locomotionController.SetVelocity(new Vector2(
                    _horizontalDirection * _horizontalForce,
                    current.y  // Maintain vertical velocity from physics
                ));
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}