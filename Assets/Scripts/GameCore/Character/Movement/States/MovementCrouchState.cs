using Common;
using GameCore.Character.Animation;

namespace GameCore.Character.Movement.States
{
    public class MovementCrouchState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Crouch;
        public override MovementStateType Type => MovementStateType.Crouch;
        
        public MovementCrouchState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }
        
        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.IsGrounded && CrouchPressed();
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return nextState != MovementStateType.Walk || CrouchPressed();
        }

        public override void OnEnter(MovementStateType prevState)
        {
            moveValues.FloatingHeightMultiplier = parameters.crouchHeightMultiplier;
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.FloatingHeightMultiplier = 1f;
        }

        public override void FixedUpdate()
        {
            if (!movement.IsControlledByPlayer) return;
            
            var input = movement.InputState.moveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            float speed = parameters.speed * parameters.crouchSpeedMultiplier * moveValues.SpeedMultiplier;
            if (movement.InputState.sneak.IsHold())
                speed *= parameters.sneakSpeedMultiplier;

            input *= speed;
            movement.Move(input);
        }

        private bool CrouchPressed() => movement.IsControlledByPlayer && movement.InputState.crouch.IsDown();
    }
}