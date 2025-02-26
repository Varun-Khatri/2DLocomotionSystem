using System.Collections;
using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class DashStrategy : BaseStrategy
    {
        private float dashTime;
        private float dashForce;
        private Vector2 dashVelocity;
        private Coroutine dashRoutine;
        private bool isDashing;

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
            base.Enter();
            isDashing = true;
            _locomotionController.StartCoroutine(Dash());
            ((DashSettings)_settings).SetForceApplied(true);
        }

        private IEnumerator Dash()
        {
            // Freeze vertical movement during dash
            Vector2 currentVelocity = _locomotionController.GetVelocity();
            float direction = _locomotionController.FacingRight ? 1 : -1;

            // Celeste-style fixed horizontal dash
            dashVelocity = new Vector2(direction * dashForce, 0f);
            _locomotionController.SetVelocity(dashVelocity);

            // Disable gravity during dash
            _locomotionController.SetGravity(false);

            float elapsed = 0f;
            while (elapsed < dashTime)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Celeste-style momentum conservation
            _locomotionController.SetVelocity(new Vector2(dashVelocity.x * 0.7f, 0f));
            ((DashSettings)_settings).SetForceApplied(false);
            isDashing = false;
        }

        public override void PhysicsExecute()
        {
            if (isDashing)
            {
                // Maintain consistent dash velocity
                _locomotionController.SetVelocity(dashVelocity);
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (dashRoutine != null)
            {
                _locomotionController.StopCoroutine(dashRoutine);
            }
            _locomotionController.SetGravity(true);
            ((DashSettings)_settings).SetForceApplied(false);
        }
    }
}