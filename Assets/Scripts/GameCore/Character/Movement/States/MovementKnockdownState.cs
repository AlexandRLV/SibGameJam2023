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
            return moveValues.IsKnockdown;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return moveValues.KnockdownTime <= 0f;
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.IsKnockdown = false;
        }

        public override void Update()
        {
            moveValues.KnockdownTime -= Time.deltaTime;
        }
    }
}