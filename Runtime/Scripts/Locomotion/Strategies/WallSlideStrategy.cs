using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{

    public class WallSlideStrategy : BaseStrategy
    {

        private Vector2 _velocity;

        public WallSlideStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings) : base(locomotionController, inputHandler, settings)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _velocity = new Vector2(0, ((WallSlideSettings)Settings).SlideSpeed);
        }

        public override void Execute()
        {
            base.Execute();
            _locomotionController.SetVelocity(_velocity);
        }

    }
}
