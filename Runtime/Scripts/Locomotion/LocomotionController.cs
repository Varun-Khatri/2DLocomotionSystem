using System;
using System.Collections.Generic;
using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    public class LocomotionController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<BaseSettings> _settingsList;
        [SerializeField] private BaseSettings _defaultStrategy;
        [SerializeField] private LocomotionSettings _locomotionSettings;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] bool _applyGravity;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private bool _facingRight;
        [SerializeField] private bool _enableGizmos;

        private List<BaseStrategy> _strategies = new List<BaseStrategy>();
        private BaseStrategy _currentStrategy;

        private Rigidbody2D _rb;
        private bool _isGrounded;
        private bool _isTouchingWall;
        private bool _inCoyoteTime;
        private float _coyoteTimeCounter;
        private bool _cachedApplyGravity;

        public bool ApplyGravity => _applyGravity;
        public bool IsGrounded => _isGrounded;
        public bool IsTouchingWall => _isTouchingWall;
        public bool InCoyoteTime => _inCoyoteTime;
        public bool FacingRight => _facingRight;

        public LocomotionSettings LocomotionSettings => _locomotionSettings;
        public bool IsDashing => _currentStrategy is DashStrategy;
        public bool IsClimbing => _currentStrategy is WallClimbStrategy;
        public Quaternion PlayerRotation => transform.rotation;
        public Action<BaseStrategy> OnStrategyChanged;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _facingRight = true;
            InitializeComponents();
            if (_cachedApplyGravity != _applyGravity)
            {
                _cachedApplyGravity = _applyGravity;
            }
            _rb.gravityScale = _applyGravity ? _locomotionSettings.gravityScale : 0;
        }

        private void InitializeComponents()
        {
            foreach (var strategySettings in _settingsList)
            {
                //Instantiate the strategy
                var strategy = strategySettings.GetStrategy(this, _inputHandler);
                _strategies.Add(strategy);
                if (strategy.Settings == _defaultStrategy)
                {
                    SetStrategy(strategy);
                }
            }
        }

        private void Update()
        {
            if (_cachedApplyGravity != _applyGravity)
            {
                _cachedApplyGravity = _applyGravity;
                _rb.gravityScale = _applyGravity ? _locomotionSettings.gravityScale : 0;
            }
            if (_applyGravity)
            {
                CheckGrounded();
                UpdateCoyoteTime();
                CheckWallTouch();
            }

            if (_currentStrategy.Settings.ExitCondition())
            {
                foreach (var locoSettings in _currentStrategy.TransitionsTo)
                {
                    BaseStrategy containerTo = _strategies.Find(strategy => strategy.Settings == locoSettings);
                    if (containerTo == null)
                        continue;

                    if (containerTo.EnterCondition())
                    {
                        SetStrategy(containerTo);
                        break;
                    }
                }
            }
            if (_currentStrategy != null)
            {
                _currentStrategy.Execute();
            }
        }

        private void FixedUpdate()
        {
            if (_currentStrategy != null)
            {
                _currentStrategy.PhysicsExecute();
            }
        }
        private void UpdateCoyoteTime()
        {
            if (_isGrounded)
            {
                _coyoteTimeCounter = _locomotionSettings.coyoteTime; // Reset coyote time when grounded
                _inCoyoteTime = true;
            }
            else if (_inCoyoteTime)
            {
                _coyoteTimeCounter = Mathf.Max(_coyoteTimeCounter - Time.deltaTime, 0); // Decrease coyote time only if in coyote time
                _inCoyoteTime = _coyoteTimeCounter > 0; // Update coyote time status
            }
        }

        public void ResetCoyoteTime()
        {
            _coyoteTimeCounter = 0; // Immediately reset coyote time
            _inCoyoteTime = false; // Ensure in-coyote-time status is updated
        }

        private void CheckGrounded()
        {
            // Check if the player is grounded by overlapping a circle at the ground check position
            _isGrounded = Physics2D.OverlapCircle(
                _groundCheck.position, // Position to check
            _locomotionSettings.groundCheckRadius, // Radius of the check
                _locomotionSettings.groundLayer // Layer mask to filter which layers count as ground
            );

        }

        private void SetStrategy(BaseStrategy strategy)
        {
            if (strategy == _currentStrategy)
                return;

            _currentStrategy?.Exit();


            _currentStrategy = strategy;
            _currentStrategy.Enter();

            OnStrategyChanged?.Invoke(_currentStrategy);
            Debug.Log($"Strategy Changed to {strategy.GetType().Name}");
        }


        private void CheckWallTouch()
        {
            _isTouchingWall = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, _locomotionSettings.wallCheckDistance, _locomotionSettings.wallLayer);
        }

        public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            if (_rb != null)
            {
                _rb.AddForce(force, forceMode);
            }
        }

        public void SetVelocityY(float value)
        {
            _rb.linearVelocityY = value;
        }
        public void SetVelocityX(float value)
        {
            _rb.linearVelocityX = value;
        }

        public void SetRotation(Quaternion rotation) => transform.rotation = rotation;
        public void SetFacing(bool facingRight) => _facingRight = facingRight;
        public void SetVelocity(Vector2 vector) => _rb.linearVelocity = vector;
        public Vector2 GetVelocity() => _rb.linearVelocity;
        public float GravityScale
        {
            get
            {
                return _rb.gravityScale;
            }
            set
            {
                _rb.gravityScale = value;
            }
        }
        public T GetSettings<T>() where T : BaseSettings
        {
            foreach (var settings in _settingsList)
            {
                if (settings is T specificSettings)
                {
                    return specificSettings;
                }
            }
            throw new System.ArgumentException($"Settings of type {typeof(T)} not found.");
        }

        public void EnableGravity(bool value)
        {
            _applyGravity = value;
        }

        public Vector2 GetWallNormal()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, _locomotionSettings.wallCheckDistance, _locomotionSettings.wallLayer);
            return hit.normal;
        }

        private void OnDrawGizmos()
        {
            if (!_enableGizmos) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_groundCheck.position, _locomotionSettings.groundCheckRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + ((transform.right * transform.localScale.x) * _locomotionSettings.wallCheckDistance));

        }
    }
}