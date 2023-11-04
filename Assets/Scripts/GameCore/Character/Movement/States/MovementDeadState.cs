using GameCore.Character.Animation;

namespace GameCore.Character.Movement.States
{
    public class MovementDeadState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Dead;
        public override MovementStateType Type => MovementStateType.Dead;
        
        public MovementDeadState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.IsDead;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return !movement.IsDead;
        }
    }
}