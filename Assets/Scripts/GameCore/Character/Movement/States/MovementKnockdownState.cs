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
            movement.KnockdownEffect.SetActive(true);
            movement.KnockdownEffect.GetComponent<ParticleSystem>().Play();
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.IsKnockdown = false;
            movement.KnockdownEffect.SetActive(false);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
        }
    }
}