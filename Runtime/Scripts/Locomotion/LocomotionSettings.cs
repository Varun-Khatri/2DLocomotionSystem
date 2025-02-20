using UnityEngine;

namespace VK.Locomotion
{

    [CreateAssetMenu(fileName = "LocomotionSettings", menuName = "Locomotion/Settings", order = 1)]
    public class LocomotionSettings : ScriptableObject
    {
        [Header("Physics Settings")]
        public float coyoteTime = 0.2f;
        public float gravity = -9.8f;
        public float gravityScale = 30f;
        [Header("Environment Settings")]
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.2f;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;
    }
}