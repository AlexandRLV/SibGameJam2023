using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementIdleWaitState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.IdleWait;
        public override MovementStateType Type => MovementStateType.IdleWait;

        private float _standStillAnimationTime;
        
        public MovementIdleWaitState(CharacterMovement characterMovement) : base(characterMovement)
        {
            whiteListOnEnter.Add(MovementStateType.Walk);
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            if (parameters.standStillAnimationTime == 0f || parameters.standStillTimeToStartAnimation == 0f)
                return false;
            
            return !movement.IsControlledByPlayer || movement.InputState.moveVector == Vector2.zero;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            if (nextState != MovementStateType.Walk) return true;
            if (movement.IsControlledByPlayer && movement.InputState.moveVector != Vector2.zero) return true;
            return _standStillAnimationTime > parameters.standStillAnimationTime;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _standStillAnimationTime = 0f;
        }

        public override void Update()
        {
            _standStillAnimationTime += Time.deltaTime;
        }
    }
}