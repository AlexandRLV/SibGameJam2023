using Common;
using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementWalkState : MovementStateBase
    {
        public override AnimationType AnimationType => _standStillTime > Const.Character.StandStillTime ? AnimationType.IdleWait : AnimationType.Walk;
        public override MovementStateType Type => MovementStateType.Walk;

        private float _standStillTime;

        public MovementWalkState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return movement.IsGrounded;
        }

        public override void Update()
        {
            _standStillTime += Time.deltaTime;
            if (!movement.IsControlledByPlayer) return;
            
            var input = movement.InputState.moveVector;
            if (input.magnitude > 1f)
                input = input.normalized;

            if (input != Vector2.zero)
                _standStillTime = 0f;

            float speed = parameters.speed * moveValues.SpeedMultiplier;
            if (movement.InputState.sneak.IsHold())
                speed *= parameters.sneakSpeedMultiplier;
            
            input *= speed;
            movement.Move(input);
        }
    }
}