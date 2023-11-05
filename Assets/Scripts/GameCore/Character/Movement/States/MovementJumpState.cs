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
            return !movement.IsGrounded || JumpPressed();
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _isJumping = false;
            if (!JumpPressed() || !parameters.canJump) return;
            
            float jumpForce = Mathf.Sqrt(-2f
                                         * Physics.gravity.y
                                         * parameters.jumpHeight * moveValues.JumpHeightMultiplier
                                         * parameters.gravityMultiplier
                                         * 1.1f) * rigidbody.mass;
            
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpTimer = parameters.jumpTime;
            _isJumping = true;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return nextState != MovementStateType.Walk || _jumpTimer < 0f;
        }

        public override void Update()
        {
            _jumpTimer -= Time.deltaTime;
            if (_jumpTimer < 0f)
                _isJumping = false;
            
            if (!movement.IsControlledByPlayer) return;
            var input = movement.InputState.moveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            input *= parameters.inAirSpeed;
            
            movement.MoveInAir(input);
        }

        private bool JumpPressed() => movement.IsControlledByPlayer && movement.InputState.jump.IsDown();
    }
}