using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class WallClimbStrategy : BaseStrategy
    {
        private float _wallClimbSpeed;
        private Vector2 _moveInput;
        private Vector2 _velocity;

        public WallClimbStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {

            if (settings is WallClimbSettings wallClimbingSettings)
            {
                _wallClimbSpeed = wallClimbingSettings.wallClimbSpeed;
            }
        }
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entering Wall Climb Strategy");

        }

        public override void Execute()
        {
            base.Execute();
            _moveInput = _inputHandler.MovementInput;
            _velocity = _locomotionController.GetVelocity();
            if (_moveInput.y != 0)
            {
                _velocity = new Vector2(_velocity.x, _wallClimbSpeed);
                _locomotionController.SetVelocity(_velocity);
                Debug.Log(($"Wall Climb Velocity: {_locomotionController.GetVelocity()}"));
            }
        }
    }
}
