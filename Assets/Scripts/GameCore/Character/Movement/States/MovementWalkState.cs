using Common;
using GameCore.Character.Animation;

namespace GameCore.Character.Movement.States
{
    public class MovementWalkState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Walk;
        public override MovementStateType Type => MovementStateType.Walk;

        public MovementWalkState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.IsGrounded;
        }

        public override void Update()
        {
            if (!movement.IsControlledByPlayer) return;
            
            var input = movement.InputState.moveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            float speed = parameters.speed * moveValues.SpeedMultiplier;
            if (movement.InputState.sneak.IsHold())
                speed *= parameters.sneakSpeedMultiplier;
            
            input *= speed;
            movement.Move(input);
        }
    }
}