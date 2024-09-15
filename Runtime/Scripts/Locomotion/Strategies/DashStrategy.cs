using System.Collections;
using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class DashStrategy : BaseStrategy
    {
        private float dashTime;
        private float dashForce;
        private Vector2 _velocity;

        public DashStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
            : base(locomotionController, inputHandler, settings)
        {
            if (settings is DashSettings dashingSettings)
            {
                dashForce = dashingSettings.dashForce;
                dashTime = dashingSettings.dashTime;
            }
        }

        public override void Enter()
        {
            // Perform dashing logic
            Debug.Log("Entered Dash");
            _velocity = _locomotionController.GetVelocity();
            _locomotionController.StartCoroutine(Dash());
        }

        private IEnumerator Dash()
        {
            float directionFacing = _locomotionController.FacingRight ? 1 : -1;
            _velocity = new Vector2(directionFacing * dashForce, _velocity.y);
            _locomotionController.SetVelocity(_velocity);
            yield return new WaitForSecondsRealtime(dashTime);
            ((DashSettings)_settings).SetForceApplied(true);
        }

        public override void Exit()
        {
            base.Exit();
            ((DashSettings)_settings).SetForceApplied(false);
            Debug.Log("Exited Dash");
        }

    }
}