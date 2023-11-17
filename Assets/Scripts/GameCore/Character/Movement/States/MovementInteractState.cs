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
            if (!movement.InputState.interact.IsDown() && !moveValues.ForceInteract) return false;

            return moveValues.CurrentInteractiveObject != null;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _animationType = moveValues.CurrentInteractiveObject.InteractAnimation;
            moveValues.ForceInteract = false;
            _timer = parameters.interactTime;
            moveValues.CurrentInteractiveObject.Interact();
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
        }
    }
}