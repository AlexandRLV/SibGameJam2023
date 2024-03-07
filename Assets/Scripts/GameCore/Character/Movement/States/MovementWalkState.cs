using GameCore.Character.Animation;
using GameCore.LevelObjects.TriggerObjects;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementWalkState : MovementStateBase
    {
        public override AnimationType AnimationType => _isPushing ? AnimationType.Push : AnimationType.Walk;
        public override MovementStateType Type => MovementStateType.Walk;

        private float _standStillTime;
        private bool _isPushing;

        public MovementWalkState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return moveValues.IsGrounded;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return nextState != MovementStateType.IdleWait ||
                   _standStillTime > parameters.standStillTimeToStartAnimation;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _standStillTime = 0f;
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
            
            input *= speed;
            movement.Move(input);

            if (!parameters.canPush)
            {
                _isPushing = false;
                return;
            }

            if (!Physics.Raycast(movement.transform.position + Vector3.up, movement.transform.forward, out var hit, 1f))
            {
                _isPushing = false;
                return;
            }

            var pushable = hit.collider.GetComponent<PushableObject>();
            _isPushing = pushable != null;
        }
    }
}