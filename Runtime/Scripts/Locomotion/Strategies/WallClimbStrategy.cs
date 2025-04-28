using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class WallClimbStrategy : BaseStrategy
    {
        private float _wallClimbSpeed;
        private Vector2 _moveInput;
        private Vector2 _velocity;
        private float _timer;
        private float _duration;

        public WallClimbStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {
            if (settings is WallClimbSettings wallClimbingSettings)
            {
                _wallClimbSpeed = wallClimbingSettings.WallClimbSpeed;
                _duration = wallClimbingSettings.Duration;
            }
        }

        public override void Enter()
        {
            base.Enter();
            ((WallClimbSettings)Settings).CanSlide(false);
            _duration = ((WallClimbSettings)Settings).Duration;
            _timer = 0f; // Reset timer when entering the strategy
        }

        public override void Execute()
        {
            base.Execute();
            _moveInput = _inputHandler.MovementInput;
            _velocity = _locomotionController.GetVelocity();
            if (_moveInput.y != 0)
            {
                _timer = 0f; // Reset timer when there's vertical input
                _velocity = new Vector2(_velocity.x, _wallClimbSpeed);
                _locomotionController.SetVelocity(_velocity);
            }
            else
            {
                _timer += Time.deltaTime; // Increment timer if no vertical input
                if (_timer >= _duration)
                {
                    ((WallClimbSettings)Settings).CanSlide(true);
                }
            }
        }
    }
}
