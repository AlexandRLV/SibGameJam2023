using Common;
using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementJumpState : MovementStateBase
    {
        public override AnimationType AnimationType => _isJumping ? AnimationType.Jump : AnimationType.Fall;
        public override MovementStateType Type => MovementStateType.Jump;

        private bool _isJumping;
        private float _jumpTimer;
        
        public MovementJumpState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return !movement.IsGrounded || movement.InputState.JumpPressed;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            float jumpForce = Mathf.Sqrt(-2f * Const.Character.Gravity * parameters.jumpHeight * 1.1f);
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpTimer = Const.Character.MinJumpTime;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return nextState != MovementStateType.Walk || _jumpTimer < 0f;
        }

        public override void FixedUpdate()
        {
            _jumpTimer -= Time.deltaTime;
            
            var input = movement.InputState.MoveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            input *= parameters.inAirSpeed;
            
            movement.Move(input);
        }
    }
}