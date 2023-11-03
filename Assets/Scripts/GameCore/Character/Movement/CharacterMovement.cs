using Common;
using GameCore.Input;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        public bool IsDead { get; private set; }
        public InputState InputState { get; private set; }
        public Rigidbody Rigidbody => _rigidbody;
        public CharacterParameters Parameters => _parameters;
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterParameters _parameters;

        private void Awake()
        {
            InputState = GameContainer.InGame.Resolve<InputState>();
        }

        private void FixedUpdate()
        {
            // TODO: get grounded
            // TODO: calculate floating
        }

        public void Move(Vector2 input)
        {
            var movement = _rigidbody.rotation * new Vector3(input.x, 0f, input.y);
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
        }

        public void MoveInAir(Vector2 input)
        {
            var movement = _rigidbody.rotation * new Vector3(input.x, 0f, input.y);
            var horizontalVelocity = _rigidbody.velocity;
            float verticalVelocity = horizontalVelocity.y;
            horizontalVelocity.y = 0f;

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
            _rigidbody.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
        }
    }
}