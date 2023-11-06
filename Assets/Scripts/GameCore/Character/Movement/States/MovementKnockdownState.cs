using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementKnockdownState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Knockdown;
        public override MovementStateType Type => MovementStateType.Knockdown;

        private float _timer;
        
        public MovementKnockdownState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return moveValues.IsKnockdown;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _timer = parameters.knockdownTime;
            if (movement.KnockdownEffect != null)
                movement.KnockdownEffect.SetActive(true);
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.IsKnockdown = false;
            if (movement.KnockdownEffect != null)
                movement.KnockdownEffect.SetActive(true);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
        }
    }
}