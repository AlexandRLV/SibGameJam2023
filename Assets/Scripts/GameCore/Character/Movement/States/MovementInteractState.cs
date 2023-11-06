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

            return movement.MoveValues.CurrentInteractiveObject != null;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _animationType = movement.MoveValues.CurrentInteractiveObject.InteractAnimation;

            _timer = parameters.interactTime;
            movement.MoveValues.CurrentInteractiveObject.Interact();
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
        }
    }
}