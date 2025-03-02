using System.Collections;
using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class DashStrategy : BaseStrategy
    {
        // Configuration
        private readonly float _dashSpeed;
        private readonly float _dashDuration;
        private readonly float _cooldown;

        // State
        private Vector2 _dashDir;
        private bool _isDashing;
        private float _lastDashTime = -Mathf.Infinity;
        private Coroutine _activeDash;

        public DashStrategy(LocomotionController controller,
                           InputHandler inputHandler,
                           DashSettings settings)
            : base(controller, inputHandler, settings)
        {
            _dashSpeed = settings.dashDistance / settings.dashTime;
            _dashDuration = settings.dashTime;
            _cooldown = settings.cooldown;
        }

        public override void Enter()
        {
            if (!CanDash()) return;

            base.Enter();

            ((DashSettings)Settings).SetForceApplied(false);
            _isDashing = true;
            _lastDashTime = Time.time;
            _activeDash = _locomotionController.StartCoroutine(PerformDash());
        }

        private IEnumerator PerformDash()
        {
            // Determine direction
            Vector2 input = _inputHandler.MovementInput.normalized;
            _dashDir = GetDashDirection(input);

            // Freeze physics
            _locomotionController.EnableGravity(false);
            _locomotionController.SetVelocityY(0f);

            // Dash movement
            float elapsed = 0f;
            while (elapsed < _dashDuration)
            {

                _locomotionController.SetVelocity(_dashDir * _dashSpeed);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Transition out
            EndDash();
        }

        private Vector2 GetDashDirection(Vector2 input)
        {
            // 8-directional input detection
            if (input.sqrMagnitude < 0.1f)
            {
                return _locomotionController.FacingRight ? Vector2.right : Vector2.left;
            }

            // Snap to 8 directions
            float angle = Mathf.Atan2(input.y, input.x);
            float snappedAngle = Mathf.Round(angle / (Mathf.PI / 4)) * (Mathf.PI / 4);
            return new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle));
        }


        private void EndDash()
        {
            _isDashing = false;

            // Restore physics
            _locomotionController.EnableGravity(true);

            // Preserve horizontal momentum if moving in dash direction
            Vector2 currentVelocity = _locomotionController.GetVelocity();
            currentVelocity.x *= 0.7f;
            _locomotionController.SetVelocity(currentVelocity);
            ((DashSettings)Settings).SetForceApplied(true);
        }

        private bool CanDash()
        {
            return Time.time > _lastDashTime + _cooldown && !_isDashing;
        }

        public override void Exit()
        {
            base.Exit();
            if (_activeDash != null)
            {
                _locomotionController.StopCoroutine(_activeDash);
                EndDash();
            }
        }
    }
}