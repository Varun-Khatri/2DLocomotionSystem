using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{
    [CreateAssetMenu(fileName = "CrouchingSettings", menuName = "Locomotion/CrouchingSettings")]
    public class CrouchSettings : BaseSettings
    {
        [SerializeField] private float _crouchHeight = 1f;
        [SerializeField] private Vector2 _crouchOffset = new Vector2(0, -0.5f);
        [SerializeField] private float _moveSpeed = 3f;

        public float CrouchHeight => _crouchHeight;
        public Vector2 CrouchOffset => _crouchOffset;
        public float MoveSpeed => _moveSpeed;

        public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
        {
            throw new System.NotImplementedException();
        }
    }
}
