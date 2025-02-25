using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class IdleStrategy : BaseStrategy
    {
        private Vector3 _velocity;

        public IdleStrategy(BaseSettings settings, LocomotionController controller, InputHandler inputHandler) : base(controller, inputHandler, settings)
        {
            _settings = settings;
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void PhysicsExecute()
        {
            base.PhysicsExecute();
            if (!_locomotionController.ApplyGravity) return;
            _velocity = _locomotionController.GetVelocity();
            _velocity.y = _locomotionController.LocomotionSettings.gravity;
            _locomotionController.SetVelocity(_velocity * Time.deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
        }


    }
}
