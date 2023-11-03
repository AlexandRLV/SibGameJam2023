using GameCore.Character.Animation;

namespace GameCore.Character.Movement.States
{
    public class MovementWalkState : MovementStateBase
    {
        public override AnimationType AnimationType => _isMoving ? AnimationType.Walk : AnimationType.Idle;
        public override MovementStateType Type => MovementStateType.Walk;

        private bool _isMoving;
        
        public MovementWalkState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.IsGrounded;
        }

        public override void FixedUpdate()
        {
            var input = movement.InputState.MoveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            input *= parameters.speed;
            
            movement.Move(input);
        }
    }
}