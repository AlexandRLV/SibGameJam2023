using Common;
using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    internal class MovementInteractState : MovementStateBase
    {
        public override AnimationType AnimationType => _animationType;
        public override MovementStateType Type => MovementStateType.Interact;

        private AnimationType _animationType;
        private float _timer;
        
        public MovementInteractState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            if (!movement.IsControlledByPlayer) return false;
            if (!movement.InputState.interact.IsDown()) return false;

            return movement.MoveValues.CurrentInteractable != null;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _animationType = movement.MoveValues.CurrentInteractable.InteractAnimation;

            _timer = parameters.interactTime;
            movement.MoveValues.CurrentInteractable.Interact();
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            if (movement.IsGrounded)
                movement.Move(Vector2.zero);
            else
                movement.MoveInAir(Vector2.zero);
        }
    }
}