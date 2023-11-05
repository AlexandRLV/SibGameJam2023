using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementKnockdownState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Knockdown;
        public override MovementStateType Type => MovementStateType.Knockdown;
        
        public MovementKnockdownState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.MoveValues.IsKnockdown;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return movement.MoveValues.KnockdownTime <= 0f;
        }

        public override void OnExit(MovementStateType nextState)
        {
            movement.MoveValues.IsKnockdown = false;
        }

        public override void Update()
        {
            movement.MoveValues.KnockdownTime -= Time.deltaTime;
        }
    }
}